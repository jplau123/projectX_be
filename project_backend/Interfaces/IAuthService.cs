using project_backend.Model;
using project_backend.Model.Entities;
using System.Security.Claims;

namespace project_backend.Interfaces
{
    public interface IAuthService
    {
        public Task<UserAuth> GetUserAuthDetails(string userName);
        public Task<string> SetRefreshToken(UserAuth user);
        public Task<string> SetAccessToken(UserAuth user);
        public RefreshToken GenerateRefreshToken();
        public void SetAccessTokenCookie();
        public void SetRefreshTokenCookie();
        public Task RevokeRefreshToken();
        public void DeleteAccessTokenCookie();
        public void DeleteRefreshTokenCookie();
        public string GetAccessTokenFromCookie();
        public string GetRefreshTokenFromCookie();
        public void SetRefreshToken(string refreshToken);
        public void SetAccessToken(string accessToken);
        public bool IsTokenValid(UserAuth user, string refreshToken);
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
        public Task<UserAuth> GetUserFromToken(string token);
    }
}
