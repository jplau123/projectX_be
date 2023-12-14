﻿using Dapper;
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
            string query = "SELECT * FROM users WHERE is_deleted = false";
            var result = await _connectionString.QueryAsync<User>(query);
            return result.ToList();
        }

        public async Task<User> GetUserByUserIdAsync(int id)
        {
            string query = "SELECT * FROM users WHERE user_id = @id";
            var queryArguments = new
            {
                id
            };

            return await _connectionString.QuerySingleAsync<User>(query, queryArguments);
        }

        public async Task<int> DeleteUserByUserIdAsync(int id)
        {
            string query = "UPDATE users SET is_deleted = @isDeleted WHERE user_id = @id";
            var queryArguments = new
            {
                isDeleted = true,
                id
            };

            return await _connectionString.ExecuteAsync(query, queryArguments);
        }
    }
}
