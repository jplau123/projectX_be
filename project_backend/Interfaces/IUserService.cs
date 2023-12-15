using project_backend.DTOs.RequestDTO;
using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserService
    {
        public int AddUserBalance(int user_id, int balance);

        Task<List<User>>? GetUsersAsync();

        Task<User> GetUserByUserIdAsync(int id);

        Task DeleteUserByUserIdAsync(int id);

        Task AddUserAsync(AddUserRequest request);

        Task<User> UpdateUserByUserIdAsync(UpdateUserRequest request);
    }
}
