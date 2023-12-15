namespace project_backend.DTOs.RequestDTO
{
    public class UpdateUserRequest
    {
        public int User_Id { get; set; }

        public string User_Name { get; set; }

        public string Role { get; set; }

        public bool Active { get; set; }

        public string Modified_By { get; set; }
    }
}
