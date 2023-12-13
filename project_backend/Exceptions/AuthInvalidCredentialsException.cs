namespace project_backend.Exceptions
{
    public class AuthInvalidCredentialsException : Exception
    {
        public AuthInvalidCredentialsException(string message) : base(message) { }
    }
}
