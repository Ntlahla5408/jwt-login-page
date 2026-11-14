using JWT.Data;
using JWT.Entities;
using Microsoft.AspNetCore.Authorization;
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
        private static readonly string[] ValidRoles = { "Admin", "Doctor", "Assistant" };
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext dbContext;

        public AuthController(
            ApplicationDbContext dbContext,
            IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var username = loginDto.Username.Trim();
            var user = await dbContext.Users
                .FirstOrDefaultAsync(existingUser => existingUser.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                username = user.Username,
                role = user.Role
            });
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(AddUserDto addUser)
        {
            if (string.IsNullOrWhiteSpace(addUser.Username) ||
                string.IsNullOrWhiteSpace(addUser.Email) ||
                string.IsNullOrWhiteSpace(addUser.Password) ||
                string.IsNullOrWhiteSpace(addUser.ConfirmPassword))
            {
                return BadRequest("Username, email and password are required.");
            }

            if (addUser.Password.Length < 12)
            {
                return BadRequest("Password must contain at least 12 characters.");
            }

            if (!string.Equals(addUser.Password, addUser.ConfirmPassword, StringComparison.Ordinal))
            {
                return BadRequest("Passwords do not match.");
            }

            var role = addUser.Role?.Trim();
            if (string.IsNullOrWhiteSpace(role) ||
                !ValidRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest("Role must be Admin, Doctor, or Assistant.");
            }

            var username = addUser.Username.Trim();
            var email = addUser.Email.Trim();

            var accountExists = await dbContext.Users.AnyAsync(user =>
                user.Username == username || user.Email == email);

            if (accountExists)
            {
                return Conflict("A user with that username or email already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(addUser.Password),
                Role = ValidRoles.First(validRole =>
                    string.Equals(validRole, role, StringComparison.OrdinalIgnoreCase))
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully.",
                userId = user.Id,
                username = user.Username,
                role = user.Role
            });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
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
    }
}
