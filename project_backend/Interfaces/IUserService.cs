namespace project_backend.Interfaces
{
    public interface IUserService
    {
        public int AddUserBalance(int userId, int balance);
        public void PurchaseItem(int userId, string itemName, int quantityToBuy);
    }
}
