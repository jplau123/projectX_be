namespace project_backend.Exceptions
{
    public class ItemAlreadyExistsException : Exception
    {
        public ItemAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
