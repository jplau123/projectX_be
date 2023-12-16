using project_backend.DTOs.RequestDTO;
using project_backend.DTOs.ResponseDTO;
using project_backend.Exceptions;
using project_backend.Extensions;
using project_backend.Interfaces;
using project_backend.Model.Entities;

namespace project_backend.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public LoginService(
            IUserAuthRepository userAuthRepository,
            IUserRepository userRepository,
            IAuthService authService)
        {
            _userAuthRepository = userAuthRepository;
            _userRepository = userRepository;
            _authService = authService;
        }

        public async Task<TokenResponse> Authenticate(UserAuthRequest authRequest)
        {
            UserAuth user = await _authService.GetUserAuthDetails(authRequest.UserName);

            if (!user.Active)
                throw new AuthenticationException("User is disabled.");

            if (!authRequest.Password.BcryptVerify(user.Password))
                throw new AuthenticationException("Incorect username or password.");

            string accessToken = await _authService.SetAccessToken(user);
            string refreshToken = await _authService.SetRefreshToken(user);

            TokenResponse tokenDTO = new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            _authService.SetAccessTokenCookie();
            _authService.SetRefreshTokenCookie();

            return tokenDTO;
        }

        public async Task<User> Register(NewUserRequest request)
        {
            bool userNameExists = await _userAuthRepository.UsernameExists(request.UserName);

            if (userNameExists)
                throw new BadRequestException("User already exists.");

            string passwordHash = request.Password.Bcrypt();

            int? userId = await _userAuthRepository.SaveUser(request.UserName, passwordHash);

            if (userId == null || userId == 0)
                throw new Exception("Oops! Unexpected error occured during the user registration. Please try again. ");

            return await _userRepository.GetUserById((int)userId)
                ?? throw new Exception("Failed to load the user.");
        }

        public async Task<TokenResponse> RefreshAccess()
        {
            string accessToken = _authService.GetAccessTokenFromCookie();
            string refreshToken = _authService.GetRefreshTokenFromCookie();

            UserAuth user = await _authService.GetUserFromToken(accessToken);

            bool isValid = _authService.IsTokenValid(user, refreshToken);

            if (!isValid)
                throw new AuthenticationException("Token is not valid.");

            // Set new tokens
            string newAccessToken = await _authService.SetAccessToken(user);
            string newRefreshToken = await _authService.SetRefreshToken(user);

            TokenResponse tokenDTO = new()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };

            _authService.SetAccessTokenCookie();
            _authService.SetRefreshTokenCookie();

            return tokenDTO;
        }

        public async Task RevokeAccess()
        {
            await _authService.RevokeRefreshToken();

            _authService.DeleteAccessTokenCookie();
            _authService.DeleteRefreshTokenCookie();
        }
    }
}
