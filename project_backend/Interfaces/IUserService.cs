using project_backend.DTOs.RequestDTO;
using project_backend.DTOs.ResponseDTO;
using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserService
    {
        decimal AddUserBalance(int userId, decimal balance);

        void PurchaseItem(int userId, string itemName, int quantityToBuy);

        int AddUserBalance(int user_id, int balance);

        Task<List<User>> GetUsersAsync();

        Task<User> GetUserByIdAsync(int id);

        Task<List<GetPurchaseResponse>> GetAllPurchaseHistoryAsync();

        Task DeleteUserByIdAsync(int id);

        Task AddUserAsync(AddUserRequest request);

        Task<User> UpdateUserByIdAsync(UpdateUserRequest request);
    }
}
