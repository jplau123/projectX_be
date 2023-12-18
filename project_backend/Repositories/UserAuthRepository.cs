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
            string sql = "SELECT COUNT(*) FROM users WHERE (user_name = @user_name AND is_deleted = false) LIMIT 1";
            return await _connection.QueryFirstAsync<int>(sql, new { user_name = userName }) > 0;
        }

        public async Task<UserAuth?> GetUserAuthDetails(string userName)
        {
            string sql = "SELECT user_id, user_name, role, password, active, token, is_deleted, token_created_at, token_expires FROM users WHERE (user_name = @user_name AND is_deleted = false) LIMIT 1";
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

        public async Task<int> SaveUserToken(UserAuth user)
        {
            string sql = "UPDATE users SET token = @token, token_created_at = @token_created, token_expires = @token_expires WHERE user_id = @user_id";

            var parameters = new
            {
                user_id = user.User_Id,
                token = user.Token,
                token_created = user.Token_Created_at,
                token_expires = user.Token_Expires,
            };

            return await _connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> ExpireUserToken(UserAuth user)
        {
            string sql = "UPDATE users SET token_expires = @token_expires WHERE user_id = @user_id";

            var parameters = new
            {
                user_id = user.User_Id,
                token_expires = DateTime.UtcNow.AddMinutes(-1),
            };

            return await _connection.ExecuteAsync(sql, parameters);
        }
    }
}
