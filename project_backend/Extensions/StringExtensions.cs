namespace project_backend.Extensions
{
    public static class StringExtensions
    {
        public static string Bcrypt(this string str) 
        { 
            return BCrypt.Net.BCrypt.HashPassword(str);
        }

        public static bool BcryptVerify(this string str, string verifyHash)
        {
            return BCrypt.Net.BCrypt.Verify(str, verifyHash);
        }
    }
}
