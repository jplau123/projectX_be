namespace project_backend.DTOs
{
    public class UserAuthRequest
    {
        public required string UserName { get; set; }
        public required string PasswordHash { get; set; }
    }
}
