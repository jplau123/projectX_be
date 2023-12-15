namespace project_backend.Exceptions
{
    public class AlreadySoftDeletedException : Exception
    {
        public AlreadySoftDeletedException(string message) : base(message)
        {
        }
    }
}
