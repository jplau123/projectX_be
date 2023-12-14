using project_backend.Middlewares;

namespace project_backend.Extensions
{
    public static class ErrorMiddlewareExtension
    {
        public static IApplicationBuilder UseErrorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorMiddleware>();
        }
    }
}
