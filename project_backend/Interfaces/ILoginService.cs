using project_backend.Model.Entities;
using project_backend.DTOs.ResponseDTO;
using project_backend.DTOs.RequestDTO;

namespace project_backend.Interfaces
{
    public interface ILoginService
    {
        /// <summary>
        /// Authenticates a user based on the provided credentials and generates access and refresh tokens.
        /// </summary>
        /// <param name="authRequest">The authentication request containing user credentials.</param>
        /// <returns>
        /// A TokenResponse containing the generated access and refresh tokens.
        /// </returns>
        public Task<TokenResponse> Authenticate(UserAuthRequest authRequest);

        /// <summary>
        /// Refreshes the access token based on the provided or cookie-stored access and refresh tokens.
        /// </summary>
        /// <param name="request">The token request containing access and refresh tokens, if provided.</param>
        /// <returns>
        /// A TokenResponse with the refreshed access and refresh tokens.
        /// </returns>
        public Task<TokenResponse> RefreshAccess(TokenRequest? request);

        /// <summary>
        /// Registers a new user based on the provided user registration request.
        /// </summary>
        /// <param name="request">The user registration request containing user details.</param>
        /// <returns>
        /// A registered user object.
        /// </returns>
        public Task<User> Register(NewUserRequest request);

        /// <summary>
        /// Revokes the current access and refresh tokens, updating the database accordingly.
        /// </summary>
        /// <returns>
        /// A Task representing the asynchronous operation.
        /// </returns>
        public Task RevokeAccess();
    }
}
