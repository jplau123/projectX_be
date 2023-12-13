namespace project_backend.Exceptions
{
    public class AuthNotFoundException : Exception
    {
        public AuthNotFoundException(string message) : base(message) { }
    }
}
