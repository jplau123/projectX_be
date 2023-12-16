using project_backend.Model;
using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserAuthRepository
    {
        public Task<UserAuth?> GetUserAuthDetails(string userName);
        public Task<int?> SaveUser(string userName, string passwordHash);
        public Task<bool> UsernameExists(string userName);
        public Task<int> SaveUserToken(UserAuth user);
        public Task<int> ExpireUserToken(UserAuth user);
    }
}