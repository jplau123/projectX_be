namespace project_backend.Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message = "") : base(message) { }
    }
}
