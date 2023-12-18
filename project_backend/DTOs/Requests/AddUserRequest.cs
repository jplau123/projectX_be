namespace project_backend.DTOs.RequestDTO
{
    public class AddUserRequest
    {
        public string User_Name { get; set; }

        public int Balance { get; set; }

        public string Role { get; set; }

        public string? Created_By { get; set; }
    }
}
