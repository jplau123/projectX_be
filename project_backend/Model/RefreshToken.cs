namespace project_backend.Model
{
    public class RefreshToken
    {
        public required string Token { get; set; }
        public required DateTimeOffset Created { get; set; }
        public required DateTimeOffset Expires { get; set; }
    }
}
