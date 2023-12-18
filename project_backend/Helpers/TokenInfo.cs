namespace project_backend.Helpers
{
    public class TokenInfo
    {
        private readonly IConfiguration _configuration;

        public TokenInfo(IConfiguration configuration, IHttpContextAccessor httpAccessor)
        {
            _configuration = configuration;
        }

        public string Issuer
        {
            get => _configuration["Jwt:Issuer"]
                ?? throw new Exception("JWT issuer was not found.");
        }

        public string Audience
        {
            get => _configuration["Jwt:Audience"]
                ?? throw new Exception("JWT audience was not found.");
        }

        public string SigningKey
        {
            get => _configuration["Jwt:Key"]
                ?? throw new Exception("JWT key was not found.");
        }

        public int AccessTokenExpires
        {
            get => _configuration.GetValue<int?>("Jwt:TokenExpires")
                ?? throw new Exception("JWT expiration date was not found.");
        }

        public int RefreshTokenExpires
        {
            get => _configuration.GetValue<int?>("Jwt:RefreshTokenExpires")
                ?? throw new Exception("JWT expiration date was not found.");
        }

        public string AccessTokenCookieName
        {
            get => _configuration["Jwt:TokenCookieName"]
                ?? throw new Exception("JWT token cookie name could not be found.");
        }

        public string RefreshTokenCookieName
        {
            get => _configuration["Jwt:RefreshTokenCookieName"]
                ?? throw new Exception("Refresh token cookie name could not be found.");
        }
    }
}
