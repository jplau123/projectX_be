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

        /// <summary>
        /// Authenticates a user based on the provided credentials and generates access and refresh tokens.
        /// </summary>
        /// <param name="authRequest">The authentication request containing user credentials.</param>
        /// <returns>
        /// A TokenResponse containing the generated access and refresh tokens.
        /// </returns>
        /// <exception cref="AuthenticationException">
        /// Thrown when authentication fails due to incorrect username, password, or disabled user account.
        /// </exception>
        /// <remarks>
        /// Attempts to retrieve user authentication details and checks for an active user account.
        /// Validates the provided password against the stored hashed password.
        /// Generates an access token and checks for a valid refresh token or generates a new one.
        /// Sets the access and refresh tokens as cookies and returns a TokenResponse.
        /// </remarks>
        /// <seealso cref="UserAuthRequest"/>
        /// <seealso cref="UserAuth"/>
        /// <seealso cref="TokenResponse"/>
        /// <seealso cref="IAuthService"/>
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

        /// <summary>
        /// Registers a new user based on the provided user registration request.
        /// </summary>
        /// <param name="request">The user registration request containing user details.</param>
        /// <returns>
        /// A registered user object.
        /// </returns>
        /// <exception cref="BadRequestException">
        /// Thrown when the specified username already exists.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when an unexpected error occurs during user registration or if user retrieval fails.
        /// </exception>
        /// <remarks>
        /// Checks if the specified username already exists; if yes, throws a BadRequestException.
        /// Hashes the user's password and saves the user to the repository.
        /// Retrieves the user by their ID and returns the registered user.
        /// </remarks>
        /// <seealso cref="NewUserRequest"/>
        /// <seealso cref="IUserAuthRepository"/>
        /// <seealso cref="IUserRepository"/>
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

        /// <summary>
        /// Refreshes the access token based on the provided or cookie-stored access and refresh tokens.
        /// </summary>
        /// <param name="request">The token request containing access and refresh tokens, if provided.</param>
        /// <returns>
        /// A TokenResponse with the refreshed access and refresh tokens.
        /// </returns>
        /// <exception cref="AuthenticationException">
        /// Thrown when the provided tokens are invalid or the token refresh fails.
        /// </exception>
        /// <remarks>
        /// Checks for the presence of access and refresh tokens in the request or retrieves them from cookies.
        /// Validates the access token and its associated refresh token.
        /// Generates new access and refresh tokens, sets them as cookies, and returns a TokenResponse.
        /// </remarks>
        /// <seealso cref="TokenRequest"/>
        /// <seealso cref="TokenResponse"/>
        /// <seealso cref="IAuthService"/>
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

        /// <summary>
        /// Revokes the current access and refresh tokens, updating the database accordingly.
        /// </summary>
        /// <returns>
        /// A Task representing the asynchronous operation.
        /// </returns>
        /// <remarks>
        /// Deletes the current access and refresh tokens and expires the associated refresh token in the database.
        /// </remarks>
        /// <seealso cref="IAuthService"/>
        public async Task RevokeAccess()
        {
            await _authService.RevokeRefreshToken();

            _authService.DeleteAccessTokenCookie();
            _authService.DeleteRefreshTokenCookie();
        }
    }
}
