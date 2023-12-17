namespace project_backend.Model
{
    public class ErrorModel
    {
        public required int Status { get; set; }
        public string? Message { get; set; }
        public string? Trace { get; set; }
    }
}
