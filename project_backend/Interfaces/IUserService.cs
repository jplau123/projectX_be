﻿using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IUserService
    {
        public int AddUserBalance(int user_id, int balance);
        public Task<User> GetUserById(int user_id);
    }
}
