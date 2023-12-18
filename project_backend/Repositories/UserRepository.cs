using Dapper;
using project_backend.Interfaces;
using project_backend.Model.Entities;
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

        public async Task<int> DeleteUserByUserIdAsync(int id)
        {
            string query = "UPDATE users SET is_deleted = @isDeleted WHERE user_id = @id";
            var queryArguments = new
            {
                isDeleted = true,
                id
            };

            return await _connection.ExecuteAsync(query, queryArguments);
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
        
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            string query = "SELECT user_id, user_name, balance, role, active, created_at, created_by, modified_at, modified_by, is_deleted FROM users WHERE is_deleted = false";
            return await _connection.QueryAsync<User>(query);
        }
        
        public async Task<User> GetUserByUserIdAsync(int id)
        {
            string query = "SELECT user_id, user_name, balance, role, active, created_at, created_by, modified_at, modified_by, is_deleted FROM users WHERE user_id = @id";
            var queryArguments = new
            {
                id
            };

            return await _connection.QuerySingleAsync<User>(query, queryArguments);
        }

        public async Task<User?> GetUserById(int user_id)
        {
            string sql = $"SELECT user_id, user_name, balance, role, active, created_at, created_by, modified_at, modified_by FROM users WHERE user_id = @userId";

            User? user = await _connection.QuerySingleOrDefaultAsync<User>(sql, new { userId = user_id });

            return user;
        }
    }
}
