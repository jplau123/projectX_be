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

            try
            {
                await _next(httpContext);
                return;
            }
            catch (Exception ex)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = $"Unexpected exception was thrown. Please try again.";
            }

            _logger.Log(LogLevel.Error, $"Error: {message}");

            httpContext.Response.StatusCode = statusCode;

            var response = new ErrorViewModel
            {
                status = statusCode,
                message = message
            };

            await httpContext.Response.WriteAsJsonAsync<ErrorViewModel>(response);

            return;
        }
    }
}
