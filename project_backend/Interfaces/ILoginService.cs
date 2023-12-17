using project_backend.Model.Entities;
using project_backend.DTOs.ResponseDTO;
using project_backend.DTOs.RequestDTO;

namespace project_backend.Interfaces
{
    public interface ILoginService
    {
        public Task<TokenResponse> Authenticate(UserAuthRequest authRequest);
        public Task<TokenResponse> RefreshAccess(TokenRequest? request);
        public Task<User> Register(NewUserRequest request);
        public Task RevokeAccess();
    }
}
