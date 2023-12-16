using System.ComponentModel.DataAnnotations;

namespace project_backend.DTOs.RequestDTO
{
    public class UserAuthRequest
    {
        [Required(ErrorMessage = "The Username is required.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "The Password is required.")]
        public required string Password { get; set; }
    }
}
