using Dapper;
using project_backend.Enums;
using project_backend.Interfaces;
using project_backend.Model.Entities;
using System.Data;

namespace project_backend.Repositories
{
    public class UserAuthRepository : IUserAuthRepository
    {
        private IDbConnection _connection;

        public UserAuthRepository(IDbConnection npgsqlConnection)
        {
            _connection = npgsqlConnection;
        }

        public async Task<bool> UsernameExists(string userName)
        {
            string sql = "SELECT COUNT(*) FROM users WHERE user_name = @user_name LIMIT 1";
            return await _connection.QueryFirstAsync<int>(sql, new { user_name = userName }) > 0;
        }

        public async Task<UserAuth?> GetUserAuthDetails(string userName)
        {
            string sql = "SELECT user_id, user_name, role, password, active FROM users WHERE user_name = @user_name LIMIT 1";
            return await _connection.QueryFirstOrDefaultAsync<UserAuth>(sql, new { user_name = userName});
        }

        public async Task<int?> SaveUser(string userName, string passwordHash)
        {
            string sql = "INSERT INTO users (user_name, password, role, created_by) VALUES (@user_name, @password, @role, @created_by) RETURNING user_id";

            var parameters = new
            {
                user_name = userName, 
                password = passwordHash,
                role = nameof(Roles.User),
                created_by = userName
            };

            return await _connection.QuerySingleOrDefaultAsync<int>(sql, parameters);
        }
    }
}
