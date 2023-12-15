using System.ComponentModel.DataAnnotations;

namespace project_backend.DTOs.Requests
{
    public class NewUserRequest
    {
        [Required(ErrorMessage = "The Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The Password is required")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "The Password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The password repeat is required")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "The Password must be at least 8 characters long.")]
        [Compare(nameof(Password), ErrorMessage = "The Password and Confirm Password must match.")]
        public string ConfirmPassword { get; set; }
    }
}
