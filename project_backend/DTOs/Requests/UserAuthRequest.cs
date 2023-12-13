namespace project_backend.DTOs.Requests
{
    public class UserAuthRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
