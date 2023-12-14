using project_backend.Exceptions;
using project_backend.Model;
using System.Net;

namespace project_backend.Middlewares
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            int statusCode;
            string message;
            string? trace;

            try
            {
                await _next(httpContext);
                return;
            }
            catch (NotFoundException ex)
            {
                statusCode = (int)HttpStatusCode.NotFound;
                message = $"{ex.Message}";
                trace = ex.StackTrace;
            }
            catch (Exception ex)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = $"{ex.Message}";
                trace = ex.StackTrace;
            }


            _logger.Log(LogLevel.Error, $"----------------------------------------");
            _logger.Log(LogLevel.Error, $"Error: {message}");
            _logger.Log(LogLevel.Error, $"Trace: {trace}");

            httpContext.Response.StatusCode = statusCode;

            var response = new ErrorViewModel
            {
                Status = statusCode,
                Error = message
            };

            await httpContext.Response.WriteAsJsonAsync(response);

            return;
        }
    }
}
