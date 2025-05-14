using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairManagerApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        
        [StringLength(100)]
        public string Email { get; set; }
        
        [Required]
        public int RoleId { get; set; }
        
        [ForeignKey("RoleId")]
        public UserRole Role { get; set; }
        
        public int? GroupId { get; set; }
        
        [ForeignKey("GroupId")]
        public Group Group { get; set; }
        
        public bool IsAdmin { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastLoginAt { get; set; }
    }
}
