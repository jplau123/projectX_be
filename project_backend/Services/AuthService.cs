using Microsoft.IdentityModel.Tokens;
using project_backend.Exceptions;
using project_backend.Helpers;
using project_backend.Interfaces;
using project_backend.Model;
using project_backend.Model.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace project_backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly TokenInfo _tokenInfo;

        private string _accessToken = null!;
        private string _refreshToken = null!;

        public AuthService(
            IUserAuthRepository userAuthRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpAccessor,
            TokenInfo tokenInfo)
        {
            _userAuthRepository = userAuthRepository;
            _userRepository = userRepository;
            _httpAccessor = httpAccessor;
            _tokenInfo = tokenInfo;
        }

        public Task<string> SetAccessToken(UserAuth user)
        {
            // Add aditional claims
            ICollection<Claim> aditionalClaims = [
                new Claim(ClaimTypes.Name, user.User_Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            ];

            var jwtToken = JwtHelper.GetJwtToken(
                user.User_Name,
                _tokenInfo.SigningKey,
                _tokenInfo.Issuer,
                _tokenInfo.Audience,
                TimeSpan.FromMinutes(_tokenInfo.AccessTokenExpires),
                aditionalClaims);

            _accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return Task.FromResult(_accessToken);
        }

        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
            }

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(_tokenInfo.RefreshTokenExpires)
            };
        }

        public async Task<string> SetRefreshToken(UserAuth user)
        {
            var token = GenerateRefreshToken();

            user.Token = token.Token;
            user.Token_Created_at = token.Created;
            user.Token_Expires = token.Expires;

            await _userAuthRepository.SaveUserToken(user);

            return _refreshToken = token.Token;
        }

        public void SetAccessTokenCookie()
        {
            if (_accessToken.IsNullOrEmpty())
                throw new Exception("Access token must be set before setting the cookie.");

            _httpAccessor.HttpContext?.Response.Cookies.Append(_tokenInfo.AccessTokenCookieName, _accessToken,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(_tokenInfo.RefreshTokenExpires),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.None
                    });
        }

        public void SetRefreshTokenCookie()
        {
            if (_refreshToken.IsNullOrEmpty())
                throw new Exception("Refresh token must be set before setting its cookie.");

            _httpAccessor.HttpContext?.Response.Cookies.Append(_tokenInfo.RefreshTokenCookieName, _refreshToken,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(_tokenInfo.RefreshTokenExpires),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None
                    });
        }
        public void DeleteAccessTokenCookie()
        {
            _httpAccessor.HttpContext?.Response.Cookies.Delete(_tokenInfo.AccessTokenCookieName);
        }

        public void DeleteRefreshTokenCookie()
        {
            _httpAccessor.HttpContext?.Response.Cookies.Delete(_tokenInfo.RefreshTokenCookieName);
        }

        public async Task RevokeRefreshToken()
        {
            string token = _httpAccessor.HttpContext?.Request.Cookies[_tokenInfo.AccessTokenCookieName]
                ?? throw new AuthenticationException("Access token could not be found.");

            UserAuth user = await GetUserFromToken(token);

            if (await _userAuthRepository.ExpireUserToken(user) == 0)
                throw new Exception("Failed to revoke the token.");
        }

        public string GetAccessTokenFromCookie()
        {
            return _httpAccessor.HttpContext?.Request.Cookies[_tokenInfo.AccessTokenCookieName]
                ?? throw new AuthenticationException("Access token could not be found.");
        }

        public string GetRefreshTokenFromCookie()
        {
            return _httpAccessor.HttpContext?.Request.Cookies[_tokenInfo.RefreshTokenCookieName]
                ?? throw new AuthenticationException("Refresh token could not be found.");
        }

        public bool IsTokenValid(UserAuth user, string refreshToken)
        {
            if (user.Token != refreshToken)
                return false;

            if (user.Token_Expires < DateTime.UtcNow)
                return false;

            return true;
        }

        public async Task<UserAuth> GetUserAuthDetails(string userName)
        {
            UserAuth user = await _userAuthRepository.GetUserAuthDetails(userName)
                ?? throw new Exception($"The user with username '{userName}' does not exist. ");
            return user;
        }

        public async Task<UserAuth> GetUserFromToken(string token)
        {
            var principal = GetPrincipalFromExpiredToken(token);

            if (principal?.Identity?.Name == null)
                throw new AuthenticationException("Operation not allowed. ");

            return await GetUserAuthDetails(principal.Identity.Name);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            try
            {
                var validator = new TokenValidationParameters
                {
                    ValidIssuer = _tokenInfo.Issuer,
                    ValidAudience = _tokenInfo.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenInfo.SigningKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };

                var principal = new JwtSecurityTokenHandler().ValidateToken(token, validator, out SecurityToken securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;

                // Chech if encription algorythm used is the one we expected it to be
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token.");

                return principal;
            }
            catch (Exception)
            {
                throw new SecurityTokenException("Invalid token.");
            }
        }

        public void SetRefreshToken(string refreshToken)
        {
            _refreshToken = refreshToken;
        }
        
        public void SetAccessToken(string accessToken)
        {
            _refreshToken = accessToken;
        }

    }
}
