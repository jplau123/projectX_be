namespace project_backend.Interfaces
{
    public interface IUserRepository
    {
        public decimal AddUserBalance(int userId, decimal balance);
        public decimal GetUserBalance(int userId);
        public void UpdateUserBalance(int userId, decimal reducedBalance);
        public void AppendPurchaseHistory(int userId, string itemName, int quantity, decimal unitPrice);

    }
}
