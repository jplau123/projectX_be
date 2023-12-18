using project_backend.Model;
using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Generates and returns an access token for the specified user, incorporating additional claims.
        /// </summary>
        /// <param name="user">The user for whom the access token is generated.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation that, upon completion, returns the generated access token as a string.
        /// </returns>
        public Task<string> SetupAccessToken(UserAuth user);

        /// <summary>
        /// Generates and sets a refresh token for the specified user, updating the user's authentication details in the repository.
        /// </summary>
        /// <param name="user">The user for whom the refresh token is generated.</param>
        /// <returns>
        /// The generated refresh token string.
        /// </returns>
        public Task<string> SetupRefreshToken(UserAuth user);

        /// <summary>
        /// Generates a new refresh token with a cryptographically secure random value,
        /// along with associated creation and expiration timestamps.
        /// </summary>
        /// <returns>
        /// A <see cref="RefreshToken"/> object containing the generated token string, creation time, and expiration time.
        /// </returns>
        public RefreshToken GenerateRefreshToken();

        /// <summary>
        /// Sets the access token as an HTTP cookie in the response.
        /// </summary>
        public void SetAccessTokenCookie();

        /// <summary>
        /// Sets the refresh token as an HTTP cookie in the response.
        /// </summary>
        public void SetRefreshTokenCookie();

        /// <summary>
        /// Revokes the refresh token associated with the current access token.
        /// </summary>
        public Task RevokeRefreshToken();

        /// <summary>
        /// Deletes the access token cookie from the HTTP response.
        /// </summary>
        public void DeleteAccessTokenCookie();

        /// <summary>
        /// Deletes the refresh token cookie from the response.
        /// </summary>
        public void DeleteRefreshTokenCookie();

        /// <summary>
        /// Retrieves the access token from the HTTP request cookies.
        /// </summary>
        /// <returns>
        /// The access token string if found in the cookies.
        /// </returns>
        public string GetAccessTokenFromCookie();

        /// <summary>
        /// Retrieves the refresh token from the request cookies.
        /// </summary>
        /// <returns>
        /// The refresh token string if found in the cookies.
        /// </returns>
        public string GetRefreshTokenFromCookie();

        /// <summary>
        /// Sets the refresh token value.
        /// </summary>
        /// <param name="refreshToken">The refresh token to be set.</param>
        public void SetRefreshToken(string refreshToken);

        /// <summary>
        /// Sets the access token value.
        /// </summary>
        /// <param name="accessToken">The access token to be set.</param>
        public void SetAccessToken(string accessToken);

        /// <summary>
        /// Checks if the provided refresh token is valid for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token validity is checked.</param>
        /// <param name="refreshToken">The refresh token to be validated.</param>
        /// <returns>
        /// <c>true</c> if the refresh token is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTokenValid(UserAuth user, string refreshToken);

        /// <summary>
        /// Retrieves user authentication details based on the provided token.
        /// </summary>
        /// <param name="token">The token from which to extract user information.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation. The result is the <see cref="UserAuth"/> object if found.
        /// </returns>
        public Task<UserAuth> GetUserFromToken(string token);

        /// <summary>
        /// Retrieves user authentication details for the specified username.
        /// </summary>
        /// <param name="userName">The username for which authentication details are requested.</param>
        /// <returns>
        /// A <see cref="UserAuth"/> object if found.
        /// </returns>
        public Task<UserAuth> GetUserAuthDetails(string userName);
    }
}
