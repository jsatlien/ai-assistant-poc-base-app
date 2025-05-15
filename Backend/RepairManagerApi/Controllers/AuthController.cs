using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepairManagerApi.Data;
using RepairManagerApi.Models;

namespace RepairManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly RepairManagerContext _context;

        public AuthController(IConfiguration configuration, RepairManagerContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginModel model)
        {
            // Validate the user against the database
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.Username == model.Username);
            
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            
            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            // Generate JWT token with user information
            var token = GenerateJwtToken(user);
            
            return new AuthResponse
            {
                Token = token,
                User = new UserModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    DisplayName = user.FullName,
                    Email = user.Email,
                    Role = user.Role.Name,
                    GroupId = user.GroupId,
                    GroupName = user.Group?.Description,
                    IsAdmin = user.IsAdmin
                }
            };
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("userId", user.Id.ToString()),
                new Claim("roleId", user.RoleId.ToString())
            };
            
            // Add GroupId claim if the user has a group
            if (user.GroupId.HasValue)
            {
                claims.Add(new Claim("groupId", user.GroupId.Value.ToString()));
            }
            
            // Add IsAdmin claim
            claims.Add(new Claim("isAdmin", user.IsAdmin.ToString()));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        // Verify the password using SHA256 hash
        private bool VerifyPassword(string password, string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hashedPassword == storedHash;
            }
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public UserModel User { get; set; }
    }
}
