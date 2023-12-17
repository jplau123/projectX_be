using Microsoft.IdentityModel.Tokens;
using project_backend.Exceptions;
using project_backend.Model;
using System.Net;

namespace project_backend.Middlewares
{
    public class ErrorMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(ILogger<ErrorMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
                return;
            }
            catch (Exception ex)
            {
                ErrorModel error = await GetExceptionResponseAsync(ex);

                await HandleException(httpContext, error);

                return;
            }
        }

        private Task<ErrorModel> GetExceptionResponseAsync(Exception exception)
        {
            int statusCode;

            switch (exception)
            {
                case AuthenticationException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case SecurityTokenException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;
                case BadRequestException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ExceededAmountException:
                    statusCode = (int)HttpStatusCode.UnprocessableContent;
                    break;
                case ExceededPriceException:
                    statusCode = (int)HttpStatusCode.UnprocessableContent;
                    break;
                case AlreadySoftDeletedException:
                    statusCode = (int)HttpStatusCode.Gone;
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            string? message = exception.Message;
            string? trace = exception.StackTrace;

            return Task.FromResult(new ErrorModel()
            {
                Status = statusCode,
                Message = message,
                Trace = trace
            });
        }

        private async Task HandleException(HttpContext httpContext, ErrorModel error)
        {
            _logger.Log(LogLevel.Error, $"----------------------------------------");
            _logger.Log(LogLevel.Error, "Status Code: {status}", error.Status);
            _logger.Log(LogLevel.Error, "Error: {message}", error.Message);
            _logger.Log(LogLevel.Error, "Trace: {trace}", error.Trace);

            httpContext.Response.StatusCode = error.Status;

            var errorView = new ErrorViewModel
            {
                Status = error.Status,
                Message = error.Message
            };

            await httpContext.Response.WriteAsJsonAsync(errorView);
        }
    }
}
