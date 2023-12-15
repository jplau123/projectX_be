using Microsoft.IdentityModel.Tokens;
using project_backend.DTOs.Requests;
using project_backend.Exceptions;
using project_backend.Extensions;
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
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpAccessor;

        private string _accessToken = null!;
        private string _refreshToken = null!;


        public AuthService(
            IUserAuthRepository userAuthRepository, 
            IConfiguration configuration, 
            IUserRepository userRepository, 
            IHttpContextAccessor httpAccessor)
        {
            _userAuthRepository = userAuthRepository;
            _configuration = configuration;
            _userRepository = userRepository;
            _httpAccessor = httpAccessor;
        }

        public async Task<UserAuth> AuthenticateAsync(UserAuthRequest authRequest)
        {
            UserAuth user = await GetUserAuthDetails(authRequest.UserName);

            if (user.Is_Deleted)
                throw new NotFoundException("User does not exists.");

            if (!user.Active)
                throw new AuthenticationException("User is disabled.");

            if (!authRequest.Password.BcryptVerify(user.Password))
                throw new AuthenticationException("Incorect username or password.");

            return user;
        }

        public Task<string> SetAccessToken(UserAuth user)
        {
            string issuer = _configuration["Jwt:Issuer"]
                ?? throw new Exception("JWT issuer was not found.");
            string audience = _configuration["Jwt:Audience"]
                ?? throw new Exception("JWT audience was not found.");
            string JwtKey = _configuration["Jwt:Key"]
                ?? throw new Exception("JWT key was not found.");
            int expires = _configuration.GetValue<int?>("Jwt:TokenExpires")
                ?? throw new Exception("JWT expiration date was not found.");

            // Add aditional claims
            ICollection<Claim> aditionalClaims = [
                new Claim(ClaimTypes.Name, user.User_Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            ];

            var jwtToken = JwtHelper.GetJwtToken(
                user.User_Name,
                JwtKey,
                issuer,
                audience,
                TimeSpan.FromMinutes(expires),
                aditionalClaims);

            _accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return Task.FromResult(_accessToken);
        }

        public async Task<User> RegisterAsync(NewUserRequest request)
        {
            bool userNameExists = await _userAuthRepository.UsernameExists(request.UserName);

            if (userNameExists)
                throw new BadRequestException("User already exists.");

            string passwordHash = request.Password.Bcrypt();

            int? userId = await _userAuthRepository.SaveUser(request.UserName, passwordHash);

            if (userId == null || userId == 0)
                throw new Exception("Oops! Unexpected error occured during the user registration. Please try again. ");

            return await _userRepository.GetUserById((int)userId) ?? throw new Exception("Failed to load the user.");
        }

        public RefreshToken GenerateRefreshToken()
        {
            int expires = _configuration.GetValue<int?>("Jwt:RefreshTokenExpires")
                ?? throw new Exception("Refresh token expiration time could not be found.");

            var randomNumber = new byte[64];

            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
            }

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(expires)
            };
        }

        public async Task<string> SetRefreshToken(UserAuth userAuth)
        {
            var token = GenerateRefreshToken();

            userAuth.Token = token.Token;
            userAuth.Token_Created_at = token.Created;
            userAuth.Token_Expires = token.Expires;

            await _userAuthRepository.SaveUserToken(userAuth);

            _refreshToken = token.Token;

            return _refreshToken;
        }

        public void SetTokenCookie()
        {
            string name = _configuration["Jwt:TokenCookieName"]
                ?? throw new Exception("JWT token cookie name could not be found.");

            // Set access token cookie expiration time the same as for refresh token
            int expires = _configuration.GetValue<int?>("Jwt:RefreshTokenExpires")
                ?? throw new Exception("JWT expiration time could not be found.");

            if (_accessToken.IsNullOrEmpty())
                throw new Exception("Jwt token must be set before setting its cookie.");

            _httpAccessor.HttpContext?.Response.Cookies.Append(name, _accessToken,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(expires),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.None
                    });
        }

        public void SetRefreshTokenCookie()
        {
            string name = _configuration["Jwt:RefreshTokenCookieName"]
                ?? throw new Exception("Refresh token cookie name could not be found.");
            int expires = _configuration.GetValue<int?>("Jwt:RefreshTokenExpires")
                ?? throw new Exception("Refresh token expiration time was not found.");

            if (_refreshToken.IsNullOrEmpty())
                throw new Exception("Refresh token must be set before setting its cookie.");

            _httpAccessor.HttpContext?.Response.Cookies.Append(name, _refreshToken,
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMinutes(expires),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None
                    });
        }
        public void DeleteTokenCookie()
        {
            string name = _configuration["Jwt:TokenCookieName"]
                ?? throw new Exception("JWT token cookie name could not be found.");

            _httpAccessor.HttpContext?.Response.Cookies.Delete(name);
        }

        public void DeleteRefreshTokenCookie()
        {
            string name = _configuration["Jwt:RefreshTokenCookieName"]
                ?? throw new Exception("Refresh token cookie name could not be found.");

            _httpAccessor.HttpContext?.Response.Cookies.Delete(name);
        }

        public async Task<string> RefreshJwtToken()
        {
            string tokenCookieName = _configuration["Jwt:TokenCookieName"]
                ?? throw new Exception("JWT token cookie name could not be found.");

            string refreshTokenCookieName = _configuration["Jwt:RefreshTokenCookieName"]
                ?? throw new Exception("Refresh token cookie name could not be found.");

            string token = _httpAccessor.HttpContext?.Request.Cookies[tokenCookieName]
                ?? throw new AuthenticationException();

            string refreshToken = _httpAccessor.HttpContext?.Request.Cookies[refreshTokenCookieName]
                ?? throw new AuthenticationException();

            var principal = GetPrincipalFromExpiredToken(token);

            if (principal?.Identity?.Name == null)
                throw new AuthenticationException("Operation not allowed. ");

            UserAuth user = await GetUserAuthDetails(principal.Identity.Name);

            if (user.Token != refreshToken)
                throw new AuthenticationException("Invalid token. ");

            if (user.Token_Expires < DateTime.UtcNow)
                throw new AuthenticationException("Token expired. ");

            _accessToken = await SetAccessToken(user);

            SetTokenCookie();

            return _accessToken;
        }

        public async Task<UserAuth> GetUserAuthDetails(string userName)
        {
            return await _userAuthRepository.GetUserAuthDetails(userName)
                ?? throw new Exception($"The user with username '{userName}' does not exist. ");
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            string issuer = _configuration["Jwt:Issuer"]
                ?? throw new Exception("JWT issuer was not found.");
            string audience = _configuration["Jwt:Audience"]
                ?? throw new Exception("JWT audience was not found.");
            string JwtKey = _configuration["Jwt:Key"]
                ?? throw new Exception("JWT key was not found.");
            int expires = _configuration.GetValue<int?>("Jwt:TokenExpires")
                ?? throw new Exception("JWT expiration date was not found.");

            var validator = new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validator, out _);
        }

    }
}
