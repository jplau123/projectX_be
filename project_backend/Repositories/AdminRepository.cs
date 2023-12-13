using Dapper;
using project_backend.Interfaces;
using project_backend.Model.Entities;
using System.Data;

namespace project_backend.Repositories
{
    public class AdminRepository(IDbConnection dbConnection) : IAdminRepository
    {
        private readonly IDbConnection _connectionString = dbConnection;

        public List<User> GetUsers()
        {
            string query = "SELECT * FROM users;";
            return _connectionString.Query<User>(query).ToList();
        }
    }
}
