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

        public async Task<User?> GetUserById(int user_id)
        {
            string sql = $"SELECT user_id, user_name, balance, role, active, created_at, created_by, modified_at, modified_by FROM users WHERE user_id = @userId";

            User? user = await _connection.QuerySingleOrDefaultAsync<User>(sql, new { userId = user_id });

            return user;
        }
    }
}
