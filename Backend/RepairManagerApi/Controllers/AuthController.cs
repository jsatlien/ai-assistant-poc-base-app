using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RepairManagerApi.Data;
using RepairManagerApi.Models;
using RepairManagerApi.Services;

namespace RepairManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;
        private readonly RepairManagerContext _context;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            TokenService tokenService,
            RepairManagerContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginModel model)
        {
            // Find the user by username or email
            var user = await _userManager.FindByNameAsync(model.Username) ?? 
                       await _userManager.FindByEmailAsync(model.Username);
            
            if (user == null)
            {
                // No legacy user migration needed anymore since we're using the same User model
                return Unauthorized(new { message = "Invalid username or password" });
                
                // No legacy user migration needed anymore
            }
            
            // Verify the password with Identity
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            
            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            
            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            
            // Load the user's group if it exists
            if (user.GroupId.HasValue)
            {
                await _context.Entry(user)
                    .Reference(u => u.Group)
                    .LoadAsync();
            }
            
            // Generate JWT token
            var token = _tokenService.GenerateJwtToken(user, roles);
            
            // Create response with user details
            return new AuthResponse
            {
                Token = token,
                User = new UserModel
                {
                    Id = int.TryParse(user.Id, out var id) ? id : 0,
                    Username = user.UserName,
                    DisplayName = user.FullName ?? user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "User",
                    GroupId = user.GroupId,
                    GroupName = user.Group?.Description,
                    IsAdmin = user.IsAdmin
                }
            };
        }

        // Legacy user migration is no longer needed since we're using the same User model
        
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterModel model)
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByNameAsync(model.Username) ?? 
                              await _userManager.FindByEmailAsync(model.Email);
            
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username or email already exists" });
            }
            
            // Create new user
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.DisplayName ?? model.Username,
                GroupId = model.GroupId,
                IsAdmin = false, // New users are not admins by default
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true // Auto-confirm for simplicity
            };
            
            // Create the user with password
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "User creation failed", errors = result.Errors });
            }
            
            // Assign default role
            var defaultRole = "User";
            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(defaultRole));
            }
            await _userManager.AddToRoleAsync(user, defaultRole);
            
            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            
            // Generate token
            var token = _tokenService.GenerateJwtToken(user, roles);
            
            // Return user info and token
            return new AuthResponse
            {
                Token = token,
                User = new UserModel
                {
                    Id = int.TryParse(user.Id, out var id) ? id : 0,
                    Username = user.UserName,
                    DisplayName = user.FullName ?? user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? defaultRole,
                    GroupId = user.GroupId,
                    IsAdmin = user.IsAdmin
                }
            };
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public int? GroupId { get; set; }
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
