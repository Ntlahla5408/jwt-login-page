namespace JWT.Entities
{
    public class AddUserDto
    {
        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public required string ConfirmPassword { get; set; }

        public required string Role { get; set; }
    }
}
