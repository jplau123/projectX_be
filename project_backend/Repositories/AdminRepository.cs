using Dapper;
using project_backend.Interfaces;
using project_backend.Model.Entities;
using System.Data;

namespace project_backend.Repositories
{
    public class AdminRepository(IDbConnection dbConnection) : IAdminRepository
    {
        private readonly IDbConnection _connectionString = dbConnection;

        public async Task<List<User>> GetUsersAsync()
        {
            string query = "SELECT * FROM users;";
            var result = await _connectionString.QueryAsync<User>(query);
            return result.ToList();
        }
    }
}
