namespace project_backend.Interfaces
{
    public interface IUserRepository
    {
        public int AddUserBalance(int user_id, int balance);
    }
}
