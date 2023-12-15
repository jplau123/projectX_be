using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserService
    {
        public int AddUserBalance(int user_id, int balance);

        Task<List<User>>? GetUsersAsync();

        Task DeleteUserByUserIdAsync(int id);
    }
}
