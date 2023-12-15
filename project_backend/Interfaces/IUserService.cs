namespace project_backend.Interfaces
{
    public interface IUserService
    {
        public int AddUserBalance(int user_id, int balance);
        public void PurchaseItem(int user_id, string item_name, int amountToBuy);
    }
}
