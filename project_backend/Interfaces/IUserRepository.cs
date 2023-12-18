using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserRepository
    {
        public decimal AddUserBalance(int userId, decimal balance);
        public decimal GetUserBalance(int userId);
        public void UpdateUserBalance(int userId, decimal reducedBalance);
        public void AppendPurchaseHistory(int userId, string itemName, int quantity, decimal unitPrice);
        public Task<IEnumerable<User>> GetUsersAsync();
        public Task<User> GetUserByUserIdAsync(int id);
        public Task<int> DeleteUserByUserIdAsync(int id);
        public Task<User?> GetUserById(int user_id);
    }
}
