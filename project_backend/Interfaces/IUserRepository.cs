namespace project_backend.Interfaces
{
    public interface IUserRepository
    {
        public int AddUserBalance(int userId, int balance);
        public int GetUserBalance(int userId);
        public void UpdateUserBalance(int userId, int reducedBalance);
        public void AppendPurchaseHistory(int userId, string itemName, int quantity, int unitPrice);

    }
}
