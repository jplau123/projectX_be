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

        public int AddUserBalance(int user_id, int balance)
        {
            string sql = $"UPDATE users SET balance = @Balance WHERE user_id = @User_Id RETURNING balance";
            var queryArguments = new
            {
                Balance = balance,
                User_Id = user_id
            };

            int newBalance = _connection.QuerySingleOrDefault<int>(sql, queryArguments, null);
            return newBalance;
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
    }
}
