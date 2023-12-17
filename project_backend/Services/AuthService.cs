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

        /// <summary>
        /// Generates and returns an access token for the specified user, incorporating additional claims.
        /// </summary>
        /// <param name="user">The user for whom the access token is generated.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation that, upon completion, returns the generated access token as a string.
        /// </returns>
        /// <remarks>
        /// This method adds specific claims, such as user ID and role, to the additionalClaims collection.
        /// It then uses the JwtHelper.GetJwtToken method to create a JWT token with the specified user details,
        /// signing key, issuer, audience, and expiration time. The resulting JWT token is written to a string format
        /// using JwtSecurityTokenHandler, and the completed access token is returned.
        /// </remarks>
        public Task<string> SetupAccessToken(UserAuth user)
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

        /// <summary>
        /// Generates and sets a refresh token for the specified user, updating the user's authentication details in the repository.
        /// </summary>
        /// <param name="user">The user for whom the refresh token is generated.</param>
        /// <returns>
        /// The generated refresh token string.
        /// </returns>
        /// <remarks>
        /// This method generates a new refresh token, assigns it to the user, updates the token-related properties
        /// (creation time and expiration), and persists the updated user authentication details to the repository.
        /// </remarks>
        public async Task<string> SetupRefreshToken(UserAuth user)
        {
            var token = GenerateRefreshToken();

            user.Token = token.Token;
            user.Token_Created_at = token.Created;
            user.Token_Expires = token.Expires;

            await _userAuthRepository.SaveUserToken(user);

            return _refreshToken = token.Token;
        }


        /// <summary>
        /// Generates a new refresh token with a cryptographically secure random 64-byte value,
        /// along with associated creation and expiration timestamps.
        /// </summary>
        /// <returns>
        /// A <see cref="RefreshToken"/> object containing the generated token string, creation time, and expiration time.
        /// </returns>
        /// <remarks>
        /// This method utilizes a <see cref="RandomNumberGenerator"/> to create a 64-byte random value,
        /// which is then converted to a Base64-encoded string. The refresh token is assigned creation and expiration timestamps
        /// based on the configured expiration duration specified in <see cref="TokenInfo"/>.
        /// The resulting <see cref="RefreshToken"/> object is returned for further use.
        /// </remarks>
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

        /// <summary>
        /// Sets the access token as an HTTP cookie in the response.
        /// </summary>
        /// <remarks>
        /// This method is responsible for setting the access token as an HTTP cookie with specified options.
        /// </remarks>
        /// <exception cref="Exception">
        /// Thrown when the access token is null or empty. The access token must be set before setting the cookie.
        /// </exception>
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

        /// <summary>
        /// Sets the refresh token as an HTTP cookie in the response.
        /// </summary>
        /// <remarks>
        /// This method is responsible for setting the refresh token as an HTTP cookie with specified options.
        /// </remarks>
        /// <exception cref="Exception">
        /// Thrown when the refresh token is null or empty. The refresh token must be set before setting its cookie.
        /// </exception>
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

        /// <summary>
        /// Deletes the access token cookie from the HTTP response.
        /// </summary>
        /// <remarks>
        /// This method removes the access token cookie from the HTTP response.
        /// </remarks>
        public void DeleteAccessTokenCookie()
        {
            _httpAccessor.HttpContext?.Response.Cookies.Delete(_tokenInfo.AccessTokenCookieName);
        }

        /// <summary>
        /// Deletes the refresh token cookie from the response.
        /// </summary>
        /// <remarks>
        /// This method removes the refresh token cookie from the HTTP response.
        /// </remarks>
        /// <seealso cref="TokenInfo"/>
        public void DeleteRefreshTokenCookie()
        {
            _httpAccessor.HttpContext?.Response.Cookies.Delete(_tokenInfo.RefreshTokenCookieName);
        }

        /// <summary>
        /// Revokes the refresh token associated with the current access token.
        /// </summary>
        /// <remarks>
        /// This method retrieves the access token from the request cookies, identifies the user, and revokes the associated refresh token.
        /// </remarks>
        /// <exception cref="AuthenticationException">
        /// Thrown when the access token cannot be found in the request cookies.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when revoking the token fails.
        /// </exception>
        public async Task RevokeRefreshToken()
        {
            string token = _httpAccessor.HttpContext?.Request.Cookies[_tokenInfo.AccessTokenCookieName]
                ?? throw new AuthenticationException("Access token could not be found.");

            UserAuth user = await GetUserFromToken(token);

            if (await _userAuthRepository.ExpireUserToken(user) == 0)
                throw new Exception("Failed to revoke the token.");
        }

        /// <summary>
        /// Retrieves the access token from the request cookies.
        /// </summary>
        /// <remarks>
        /// This method retrieves the access token from the HTTP request cookies.
        /// </remarks>
        /// <returns>
        /// The access token string if found in the cookies.
        /// </returns>
        /// <exception cref="AuthenticationException">
        /// Thrown when the access token could not be found in the request cookies.
        /// </exception>
        public string GetAccessTokenFromCookie()
        {
            return _httpAccessor.HttpContext?.Request.Cookies[_tokenInfo.AccessTokenCookieName]
                ?? throw new AuthenticationException("Access token could not be found.");
        }

        /// <summary>
        /// Retrieves the refresh token from the request cookies.
        /// </summary>
        /// <remarks>
        /// This method retrieves the refresh token from the HTTP request cookies.
        /// </remarks>
        /// <returns>
        /// The refresh token string if found in the cookies.
        /// </returns>
        /// <exception cref="AuthenticationException">
        /// Thrown when the refresh token could not be found in the request cookies.
        /// </exception>
        public string GetRefreshTokenFromCookie()
        {
            return _httpAccessor.HttpContext?.Request.Cookies[_tokenInfo.RefreshTokenCookieName]
                ?? throw new AuthenticationException("Refresh token could not be found.");
        }

        /// <summary>
        /// Checks if the provided refresh token is valid for the specified user.
        /// </summary>
        /// <remarks>
        /// This method verifies if the provided refresh token matches the user's token and if the token has not expired.
        /// </remarks>
        /// <param name="user">The user for whom the token validity is checked.</param>
        /// <param name="refreshToken">The refresh token to be validated.</param>
        /// <returns>
        /// <c>true</c> if the refresh token is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTokenValid(UserAuth user, string refreshToken)
        {
            if (user.Token != refreshToken)
                return false;

            if (user.Token_Expires < DateTime.UtcNow)
                return false;

            return true;
        }

        /// <summary>
        /// Retrieves user authentication details for the specified username.
        /// </summary>
        /// <remarks>
        /// This method asynchronously fetches user authentication details from the repository based on the provided username.
        /// </remarks>
        /// <param name="userName">The username for which authentication details are requested.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation. The result is the <see cref="UserAuth"/> object if found.
        /// </returns>
        /// <exception cref="NotFoundException">
        /// Thrown when the user with the specified username does not exist.
        /// </exception>
        public async Task<UserAuth> GetUserAuthDetails(string userName)
        {
            return await _userAuthRepository.GetUserAuthDetails(userName)
                ?? throw new NotFoundException($"User does not exist. ");
        }

        /// <summary>
        /// Retrieves user authentication details based on the provided token.
        /// </summary>
        /// <remarks>
        /// This method obtains a principal from an expired token and retrieves user authentication details using the associated username.
        /// </remarks>
        /// <param name="token">The token from which to extract user information.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation. The result is the <see cref="UserAuth"/> object if found.
        /// </returns>
        /// <exception cref="AuthenticationException">
        /// Thrown when the operation is not allowed due to an invalid or missing identity in the token.
        /// </exception>
        public async Task<UserAuth> GetUserFromToken(string token)
        {
            var principal = GetPrincipalFromExpiredToken(token);

            if (principal?.Identity?.Name == null)
                throw new AuthenticationException("Operation not allowed. ");

            return await GetUserAuthDetails(principal.Identity.Name);
        }

        /// <summary>
        /// Sets the refresh token value.
        /// </summary>
        /// <remarks>
        /// This method sets the value of the refresh token.
        /// </remarks>
        /// <param name="refreshToken">The refresh token to be set.</param>
        public void SetRefreshToken(string refreshToken)
        {
            _refreshToken = refreshToken;
        }

        /// <summary>
        /// Sets the access token value.
        /// </summary>
        /// <remarks>
        /// This method sets the value of the access token.
        /// </remarks>
        /// <param name="accessToken">The access token to be set.</param>
        public void SetAccessToken(string accessToken)
        {
            _refreshToken = accessToken;
        }

        /// <summary>
        /// Retrieves a ClaimsPrincipal from an expired token.
        /// </summary>
        /// <remarks>
        /// This method validates the provided token against specified parameters and returns a ClaimsPrincipal if the token is valid.
        /// </remarks>
        /// <param name="token">The expired access token from which to extract the ClaimsPrincipal.</param>
        /// <returns>
        /// A ClaimsPrincipal if the token is valid; otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="SecurityTokenException">
        /// Thrown when the token is invalid or the encryption algorithm used is unexpected.
        /// </exception>
        /// <seealso cref="TokenInfo"/>
        /// <seealso cref="TokenValidationParameters"/>
        /// <seealso cref="JwtSecurityTokenHandler"/>
        /// <seealso cref="SecurityAlgorithms"/>
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
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
    }
}
