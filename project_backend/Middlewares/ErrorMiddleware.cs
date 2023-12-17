using project_backend.Exceptions;
using project_backend.Model;
using System;
using System.Net;
 
namespace project_backend.Middlewares
{
    public class ErrorMiddleware : IMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await _next(httpContext);
                return;
            }
            catch (Exception ex)
            {
                ErrorModel error = await GetExceptionResponseAsync(ex);

                await HandleException(httpContext, error);

                return;
            }
        }

        private Task<ErrorModel> GetExceptionResponseAsync(Exception exeption)
        {
            int statusCode;
            string? message = exeption.Message;
            string? trace = exeption.StackTrace;
            string? method = "";
            int lineNumber = 0;

            switch (exeption)
            {
                case AuthenticationException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;
                case BadRequestException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            return Task.FromResult(new ErrorModel()
            {
                Status = statusCode,
                Message = message
            });
        }

        private async Task HandleException(HttpContext httpContext, ErrorModel error)
        {
            _logger.Log(LogLevel.Error, $"----------------------------------------");
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
