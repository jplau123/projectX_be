using Microsoft.IdentityModel.Tokens;
using project_backend.DTOs.Requests;
using project_backend.Exceptions;
using project_backend.Extensions;
using project_backend.Interfaces;
using project_backend.Model.Entities;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace project_backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserAuthRepository userAuthRepository, IConfiguration configuration)
        {
            _userAuthRepository = userAuthRepository;
            _configuration = configuration;
        }

        public async Task<string> AuthenticateAsync(UserAuthRequest authRequest)
        {
            if (string.IsNullOrEmpty(authRequest.Password)) 
                throw new AuthInvalidCredentialsException("Password is empty.");

            if (string.IsNullOrEmpty(authRequest.UserName))
                throw new AuthInvalidCredentialsException("Username is empty.");

            UserAuth userAuthInfo = await _userAuthRepository.GetUserAuthDetails(authRequest.UserName) 
                ?? throw new AuthNotFoundException("User does not exist.");

            string passwordHash = authRequest.Password.Bcrypt();

            if (string.IsNullOrEmpty(passwordHash))
                throw new Exception("Unexpected encription error occured. Please try again.");

            if (!passwordHash.BcryptVerify(userAuthInfo.Password))
                throw new AuthenticationException("Incorect username or password.");

            return GenerateToken(userAuthInfo);
        }

        public string GenerateToken(UserAuth userAuth)
        {
            string issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("JWT issuer was not found.");
            string audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("JWT audience was not found.");
            string JWTkey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT key was not found.");

            var key = Encoding.ASCII.GetBytes(JWTkey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("user_id", userAuth.User_Id.ToString()),
                new Claim(ClaimTypes.Role, userAuth.Role),
                new Claim(JwtRegisteredClaimNames.Sub, userAuth.User_Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
                Expires = DateTime.UtcNow.AddMinutes(100),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            string jwtToken = tokenHandler.WriteToken(token);

            return jwtToken;
        }

        public async Task<int> RegisterAsync(NewUserRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName))
                throw new AuthInvalidCredentialsException("Username is empty.");
            if (string.IsNullOrEmpty(request.Password))
                throw new AuthInvalidCredentialsException("Password is empty.");
            if (string.IsNullOrEmpty(request.PasswordRepeat))
                throw new AuthInvalidCredentialsException("Password repeat is empty.");

            if (request.Password != request.PasswordRepeat)
                throw new AuthInvalidCredentialsException("Both times password should be the same. ");

            bool userNameExists = await _userAuthRepository.UsernameExists(request.UserName);

            if (userNameExists)
                throw new AuthInvalidCredentialsException("User already exists.");

            string passwordHash = request.Password.Bcrypt();

            int? userId = await _userAuthRepository.SaveUser(request.UserName, passwordHash);

            if (userId == null || userId == 0)
                throw new Exception("Oops! There was an unexpected error during the user registration. Please try again. ");

            return (int)userId;
        }
    }
}
