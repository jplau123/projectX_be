using project_backend.DTOs.Requests;
using project_backend.Exceptions;
using project_backend.Extensions;
using project_backend.Helpers;
using project_backend.Interfaces;
using project_backend.Model.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace project_backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserAuthRepository userAuthRepository, IConfiguration configuration, IUserRepository userRepository)
        {
            _userAuthRepository = userAuthRepository;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<UserAuth> AuthenticateAsync(UserAuthRequest authRequest)
        {
            UserAuth userAuthInfo = await _userAuthRepository.GetUserAuthDetails(authRequest.UserName)
                ?? throw new NotFoundException("User does not exist.");

            // TODO check if user is deleted

            if (!userAuthInfo.Active)
                throw new AuthenticationException("User is not allowed.");

            if (!authRequest.Password.BcryptVerify(userAuthInfo.Password))
                throw new AuthenticationException("Incorect username or password.");

            return userAuthInfo;
        }

        public string GenerateToken(UserAuth user)
        {
            string issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("JWT issuer was not found.");
            string audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("JWT audience was not found.");
            string JwtKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT key was not found.");
            string JwtExpires = _configuration["Jwt:Expires"] ?? throw new ArgumentNullException("JWT expiration time was not found.");

            // Add aditional claims
            ICollection<Claim> aditionalClaims = [
                new Claim(ClaimTypes.Name, user.User_Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            ];

            var jwtToken = JwtHelper.GetJwtToken(user.User_Name, JwtKey, issuer, audience, TimeSpan.FromMinutes(int.Parse(JwtExpires)), aditionalClaims);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
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

        public void SetTokenCookie(string token)
        {
            // Set cookie
            HttpContext.Response.Cookies.Append("X-Token", token,
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
