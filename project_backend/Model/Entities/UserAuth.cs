using System.ComponentModel.DataAnnotations;

namespace project_backend.Model.Entities
{
    public class UserAuth
    {
        [Key]
        public int User_Id { get; set; }
        public required string User_Name { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public bool Active { get; set; }
        public string? Token { get; set; }
        public DateTimeOffset? Token_Created_at { get; set; }
        public DateTimeOffset? Token_Expires { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
