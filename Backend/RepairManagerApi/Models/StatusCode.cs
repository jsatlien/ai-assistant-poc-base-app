using System.ComponentModel.DataAnnotations;

namespace RepairManagerApi.Models
{
    public class StatusCode
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Description { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        public int SortOrder { get; set; }
    }
}
