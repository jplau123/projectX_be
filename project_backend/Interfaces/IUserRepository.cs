using project_backend.DTOs.RequestDTO;
using project_backend.DTOs.ResponseDTO;
using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserRepository
    {
        public int AddUserBalance(int user_id, int balance);

        Task<IEnumerable<User>> GetUsersAsync();

        Task<User> GetUserByIdAsync(int id);

        Task<IEnumerable<GetPurchaseResponse>> GetAllPurchaseHistoryAsync();

        Task<int> DeleteUserByIdAsync(int id);

        Task AddUserAsync(AddUserRequest request);

        Task<int> UpdateUserByIdAsync(UpdateUserRequest request);
    }
}
