﻿namespace project_backend.DTOs
{
    public class NewUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
    }
}
