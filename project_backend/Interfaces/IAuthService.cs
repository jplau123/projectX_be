using project_backend.DTOs.Requests;
using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IAuthService
    {
        public Task<string> AuthenticateAsync(UserAuthRequest authRequest);
        public Task<int> RegisterAsync(NewUserRequest request);
        public string GenerateToken(UserAuth userAuth);
    }
}
