using Microsoft.AspNetCore.Http;

namespace project_backend.Helpers
{
    public static class CookieHelper
    {
        public static void SetTokenCookie(HttpContext context, string token)
        {
            // Set cookie
            context.Response.Cookies.Append("X-Token", token,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });
        }
    }
}
