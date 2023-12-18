using project_backend.DTOs.RequestDTO;
using project_backend.DTOs.ResponseDTO;
using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserRepository
    { 
        decimal AddUserBalance(int userId, decimal balance);
        decimal GetUserBalance(int userId);
        void UpdateUserBalance(int userId, decimal reducedBalance);
        void AppendPurchaseHistory(int userId, string itemName, int quantity, decimal unitPrice);
        Task<IEnumerable<GetPurchaseResponse>> GetAllPurchaseHistoryAsync();
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<int> DeleteUserByIdAsync(int id);
        Task AddUserAsync(AddUserRequest request);
        Task<int> UpdateUserByIdAsync(UpdateUserRequest request);
    }
}
