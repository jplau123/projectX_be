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
    }
}
