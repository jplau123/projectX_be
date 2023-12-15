using project_backend.DTOs.Requests;
using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IAuthService
    {
        public Task<UserAuth> AuthenticateAsync(UserAuthRequest authRequest);
        public Task<User> RegisterAsync(NewUserRequest request);
        public string GenerateToken(UserAuth userAuth);
        public void SetTokenCookie(HttpContext context, string token);
    }
}
