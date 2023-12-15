namespace project_backend.Interfaces
{
    public interface IUserRepository
    {
        public int AddUserBalance(int user_id, int balance);
        public int GetUserBalance(int user_id);
        public void ReduceUserBalance(int user_id, int reducedBalance);

        public void AppendPurchaseHistory(int user_id, string item_name, int quantity, int unit_price);

    }
}
