using Dapper;
using project_backend.DTOs.RequestDTO;
using project_backend.DTOs.ResponseDTO;
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

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            string query = "SELECT user_id, user_name, balance, role, active, created_at, created_by, modified_at, modified_by, is_deleted FROM users WHERE is_deleted = false";
            return await _connection.QueryAsync<User>(query);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            string query = "SELECT user_id, user_name, balance, role, active, created_at, created_by, modified_at, modified_by, is_deleted FROM users WHERE user_id = @id";
            var queryArguments = new
            {
                id
            };

            return await _connection.QuerySingleAsync<User>(query, queryArguments);
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

        public async Task<IEnumerable<GetPurchaseResponse>> GetAllPurchaseHistoryAsync()
        {
            string query = "SELECT ph.purchase_id, ph.user_id, i.item_id, u.user_name, i.item_name, ph.price, ph.created_at FROM purchase_history ph INNER JOIN users u ON ph.user_id = u.user_id INNER JOIN items i ON ph.item_id = i.item_id;";
            return await _connection.QueryAsync<GetPurchaseResponse>(query);
        }

        public async Task<int> DeleteUserByIdAsync(int id)
        {
            string query = "UPDATE users SET is_deleted = @isDeleted WHERE user_id = @id";
            var queryArguments = new
            {
                isDeleted = true,
                id
            };

            return await _connection.ExecuteAsync(query, queryArguments);
        }

        public async Task AddUserAsync(AddUserRequest request)
        {
            string query = "INSERT INTO users (user_name, balance, role, active, is_deleted, created_by) VALUES (@name, @balance, @role, @active, @isDeleted, @createdBy)";

            var queryArguments = new
            {
                name = request.User_Name,
                balance = 0,
                role = request.Role,
                createdBy = request.Created_By,
                active = true,
                isDeleted = false,
            };

            await _connection.ExecuteAsync(query, queryArguments);
        }

        public async Task<int> UpdateUserByIdAsync(UpdateUserRequest request)
        {
            string query = "UPDATE users SET user_name = @userName, role = @role, active = @active, modified_by = @modifiedBy WHERE user_id = @id; ";
            var queryArguments = new
            {
                id = request.User_Id,
                userName = request.User_Name,
                role = request.Role,
                active = request.Active,
                modifiedBy = request.Modified_By,
            };

            return await _connection.ExecuteAsync(query, queryArguments);
        }
    }
}
