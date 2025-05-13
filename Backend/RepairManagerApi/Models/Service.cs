using System;
using System.ComponentModel.DataAnnotations;

namespace RepairManagerApi.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
    }
}
