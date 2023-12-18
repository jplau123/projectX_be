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
            UserAuth user;

            try
            {
                user = await _authService.GetUserAuthDetails(authRequest.UserName);
            }
            catch (NotFoundException)
            {
                throw new AuthenticationException("Incorrect username or password.");
            }

            if (!user.Active)
                throw new AuthenticationException("User is disabled.");

            if (!authRequest.Password.BcryptVerify(user.Password))
                throw new AuthenticationException("Incorrect username or password.");

            string accessToken = await _authService.SetupAccessToken(user);

            string refreshToken;

            // Check if user still has valid refresh token
            // (for the case of logging in from another device)
            // else generate new refresh token
            if (user.Token != null && user.Token_Expires > DateTime.UtcNow)
            {
                refreshToken = user.Token;
                _authService.SetRefreshToken(refreshToken);
            }
            else
            {
                refreshToken = await _authService.SetupRefreshToken(user);
            }

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

            return await _userRepository.GetUserByIdAsync((int)userId)
                ?? throw new Exception("Failed to load the user.");
        }

        public async Task<TokenResponse> RefreshAccess(TokenRequest? request)
        {
            string accessToken;
            string refreshToken;

            if (request == null
                || string.IsNullOrEmpty(request.AccessToken)
                || string.IsNullOrEmpty(request.RefreshToken))
            {
                accessToken = _authService.GetAccessTokenFromCookie();
                refreshToken = _authService.GetRefreshTokenFromCookie();
            }
            else
            {
                accessToken = request.AccessToken;
                refreshToken = request.RefreshToken;
            }

            UserAuth user = await _authService.GetUserFromToken(accessToken);

            bool isValid = _authService.IsTokenValid(user, refreshToken);

            if (!isValid)
                throw new AuthenticationException("Token is not valid.");

            // Set new tokens
            string newAccessToken = await _authService.SetupAccessToken(user);
            string newRefreshToken = await _authService.SetupRefreshToken(user);

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
