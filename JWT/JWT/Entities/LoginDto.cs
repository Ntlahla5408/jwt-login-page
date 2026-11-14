using System.ComponentModel.DataAnnotations;

namespace JWT.Entities
{
    public class LoginDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required]
        [StringLength(72, MinimumLength = 1)]
        public required string Password { get; set; }
    }
}
