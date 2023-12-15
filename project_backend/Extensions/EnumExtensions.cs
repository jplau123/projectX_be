namespace project_backend.Extensions
{
    public static class EnumExtensions
    {
        public static string Name(this Enum value)
        {
            return nameof(value);
        }
    }
}
