using project_backend.Attributes;
using System.ComponentModel.DataAnnotations;

namespace project_backend.DTOs.RequestDTO
{
    public class NewUserRequest
    {
        [Required(ErrorMessage = "The Username is required")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "The Password is required")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "The Password must be at least 8 characters long.")]
        [PasswordRequirements(ErrorMessage = "The Password must have at least one UPPERCASE letter and at least one number.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "The password repeat is required")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "The Password must be at least 8 characters long.")]
        [Compare(nameof(Password), ErrorMessage = "The Password and Confirm Password must match.")]
        public required string ConfirmPassword { get; set; }
    }
}
