﻿using project_backend.DTOs.RequestDTO;
using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserRepository
    {
        public int AddUserBalance(int user_id, int balance);

        Task<IEnumerable<User>> GetUsersAsync();

        Task<User> GetUserByUserIdAsync(int id);

        Task<int> DeleteUserByUserIdAsync(int id);

        Task AddUserAsync(AddUserRequest request);

        Task<int> UpdateUserByUserIdAsync(UpdateUserRequest request);
    }
}
