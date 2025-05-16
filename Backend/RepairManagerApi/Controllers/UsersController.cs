using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepairManagerApi.Data;
using RepairManagerApi.Models;

namespace RepairManagerApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RepairManagerContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(RepairManagerContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Group)
                .ToListAsync();
                
            // Map to response objects to exclude sensitive data
            return users.Select(u => {
                int id = 0;
                int.TryParse(u.Id, out id);
                
                var user = new User
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FullName = u.FullName,
                    RoleId = u.RoleId,
                    Role = u.Role,
                    GroupId = u.GroupId,
                    Group = u.Group,
                    IsAdmin = u.IsAdmin,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt
                    // PasswordHash is intentionally excluded
                };
                
                return user;
            }).ToList();
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Don't return the password hash
            user.PasswordHash = null;

            return user;
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // Create the user with Identity
            var result = await _userManager.CreateAsync(user, user.PasswordHash);
            
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Don't return the password hash
            user.PasswordHash = null;

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            // Get the existing user
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Update user properties
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.FullName = user.FullName;
            existingUser.RoleId = user.RoleId;
            existingUser.GroupId = user.GroupId;
            existingUser.IsAdmin = user.IsAdmin;

            // Update the user with Identity
            var result = await _userManager.UpdateAsync(existingUser);
            
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // If password is provided, update it
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                var passwordResult = await _userManager.ResetPasswordAsync(existingUser, token, user.PasswordHash);
                
                if (!passwordResult.Succeeded)
                {
                    return BadRequest(passwordResult.Errors);
                }
            }

            return NoContent();
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
