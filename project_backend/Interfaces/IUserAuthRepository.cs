﻿using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserAuthRepository
    {
        public Task<UserAuth?> GetUserAuthDetails(string userName);
        public Task<int?> SaveUser(string userName, string passwordHash);
        public Task<bool> UsernameExists(string userName);
    }
}