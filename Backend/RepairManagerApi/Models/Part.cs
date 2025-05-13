using System;
using System.ComponentModel.DataAnnotations;

namespace RepairManagerApi.Models
{
    public class Part
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(50)]
        public string SKU { get; set; }
    }
}
