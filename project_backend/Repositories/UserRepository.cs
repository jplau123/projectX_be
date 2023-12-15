using Dapper;
using project_backend.Interfaces;
using System.Data;

namespace project_backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public int AddUserBalance(int user_id, int balance)
        {
            string sql = $"UPDATE users SET balance = @Balance WHERE user_id = @User_Id RETURNING balance";
            var queryArguments = new
            {
                Balance = balance,
                User_Id = user_id
            };


            int increasedBalance = _connection.QuerySingleOrDefault<int>(sql, queryArguments, null);
            return increasedBalance;

        }

        public int GetUserBalance(int user_id)
        {
            string sql = $"SELECT balance FROM users WHERE user_id = @User_Id";
            var queryArguments = new
            {
                User_Id = user_id
            };
            int userBalance = _connection.QuerySingleOrDefault<int>(sql, queryArguments, null);
            return userBalance;
        }



        public void ReduceUserBalance(int user_id, int reducedBalance)
        {
            string sql = "UPDATE users SET balance = @Balance WHERE user_id = @User_Id RETURNING balance";
            var queryArguments = new
            {
                Balance = reducedBalance,
                User_Id = user_id
            };
            _connection.Execute(sql, queryArguments);
        }

        public void AppendPurchaseHistory(int user_id, string item_name, int quantityPurchased, int unit_price)
        {
            string sql = "INSERT INTO purchase_history (user_id, unit_price, quantity, item_id) VALUES (@User_Id, @Unit_Price, @Quantity, (SELECT item_id FROM items WHERE item_name = @Item_Name))";
            var queryArguments = new
            {
                User_Id = user_id,
                Item_Name = item_name,
                Unit_Price = unit_price,
                Quantity = quantityPurchased
            };
            _connection.Execute(sql, queryArguments);
        }
    }
}
