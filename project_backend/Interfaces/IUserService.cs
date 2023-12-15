using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserService
    {
        public decimal AddUserBalance(int userId, decimal balance);
        public void PurchaseItem(int userId, string itemName, int quantityToBuy);
        public int AddUserBalance(int user_id, int balance);
        public Task<List<User>>? GetUsersAsync();
        public Task DeleteUserByUserIdAsync(int id);
    }
}
