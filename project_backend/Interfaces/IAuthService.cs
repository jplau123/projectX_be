using project_backend.DTOs.Requests;
using project_backend.Model;
using project_backend.Model.Entities;
using System.Security.Claims;

namespace project_backend.Interfaces
{
    public interface IAuthService
    {
        public Task<UserAuth> AuthenticateAsync(UserAuthRequest authRequest);
        public Task<User> RegisterAsync(NewUserRequest request);
        public Task<UserAuth> GetUserAuthDetails(string userName);
        public Task<string> SetRefreshToken(UserAuth user);
        public Task<string> SetAccessToken(UserAuth user);
        public Task<string> RefreshJwtToken();
        public RefreshToken GenerateRefreshToken();
        public void SetTokenCookie();
        public void SetRefreshTokenCookie();
        public void DeleteRefreshTokenCookie();
        public void DeleteTokenCookie();
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
    }
}
