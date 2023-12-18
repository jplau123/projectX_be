namespace project_backend.Model
{
    public class RefreshToken
    {
        public required string Token { get; set; }
        public required DateTime Created { get; set; }
        public required DateTime Expires { get; set; }
    }
}
