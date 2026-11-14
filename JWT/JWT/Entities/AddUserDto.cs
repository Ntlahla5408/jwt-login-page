using System.ComponentModel.DataAnnotations;

namespace JWT.Entities
{
    public class AddUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Username may contain letters, numbers, dots, underscores and hyphens.")]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public required string Email { get; set; }

        [Required]
        [StringLength(72, MinimumLength = 12)]
        public required string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

        [Required]
        public required string Role { get; set; }
    }
}
