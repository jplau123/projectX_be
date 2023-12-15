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

        public decimal AddUserBalance(int userId, decimal balance)
        {

            string sql = $"UPDATE users SET balance = @Balance WHERE user_id = @User_Id RETURNING balance";
            var queryArguments = new
            {
                Balance = balance,
                User_Id = userId
            };

            decimal increasedBalance = _connection.QuerySingleOrDefault<decimal>(sql, queryArguments, null);
            return increasedBalance;

        }

        public decimal GetUserBalance(int userId)
        {
            string sql = $"SELECT balance FROM users WHERE user_id = @User_Id";
            var queryArguments = new
            {
                User_Id = userId
            };
            decimal userBalance = _connection.QuerySingleOrDefault<decimal>(sql, queryArguments, null);
            return userBalance;
        }



        public void UpdateUserBalance(int userId, decimal reducedBalance)
        {
            string sql = "UPDATE users SET balance = @Balance WHERE user_id = @User_Id RETURNING balance";
            var queryArguments = new
            {
                Balance = reducedBalance,
                User_Id = userId
            };
            _connection.Execute(sql, queryArguments);
        }

        public void AppendPurchaseHistory(int userId, string itemName, int quantityPurchased, decimal unitPrice)
        {
            string sql = "INSERT INTO purchase_history (user_id, unit_price, quantity, item_id) VALUES (@User_Id, @Unit_Price, @Quantity, (SELECT item_id FROM items WHERE item_name = @Item_Name))";
            var queryArguments = new
            {
                User_Id = userId,
                Item_Name = itemName,
                Unit_Price = unitPrice,
                Quantity = quantityPurchased
            };
            _connection.Execute(sql, queryArguments);
        }
    }
}
