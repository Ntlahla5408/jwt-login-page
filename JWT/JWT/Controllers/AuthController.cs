using JWT.Data;
using JWT.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext dbContext;

        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;

            // Add a default user for testing
            if (!dbContext.Users.Any())
            {
                dbContext.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@example.com",
                    Password = HashPassword("password"),
                    Role = "Admin"
                });

                dbContext.SaveChanges();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto logindto)
        {
            // Find user by username in database
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == logindto.Username);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // Verify password
            if (!VerifyPassword(logindto.Password, user.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);
            return Ok(new
            {
                token = token,
                username = user.Username,
                role = user.Role
            });
        }

        [HttpPost("register")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(AddUserDto addUser)
        {
            // Basic validation
            if (string.IsNullOrEmpty(addUser.Username) || string.IsNullOrEmpty(addUser.Password))
            {
                return BadRequest("Username and password are required");
            }

            if (addUser.Password != addUser.ConfirmPassword)
            {
                return BadRequest("Passwords do not match");
            }


            // Validate role
            var validRoles = new[] { "Admin", "Doctor", "Assistant" };
            if (!validRoles.Contains(addUser    .Role))
            {
                return BadRequest("Invalid role. Must be: Admin, Doctor, or Assistant.");
            }

            // Hash the password
            var passwordHash = HashPassword(addUser.Password);

            // Create user
            var user = new User
            {
                Username = addUser.Username,
                Email = addUser.Email,
                Password = passwordHash,
                Role = addUser.Role
            };

            // Save to database
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully",
                userId = user.Id,
                username = user.Username,
                role = user.Role
            });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
