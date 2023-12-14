namespace project_backend.Model.Entities
{
    public class User
    {
        public int User_Id { get; set; }

        public string User_Name { get; set; }

        public int Balance { get; set; }

        public string Role { get; set; }

        public bool Active { get; set; }

        public DateTime Created_At { get; set; }

        public string? Created_By { get; set; }

        public DateTime? Modified_At { get; set; }

        public string? Modified_By { get; set; }
    }
}

