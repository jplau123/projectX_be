namespace project_backend.Exceptions
{
    public class FailedToDeleteException : Exception
    {
        public FailedToDeleteException(string message) : base(message)
        {
        }
    }
}
