using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        
        [Required]
        [StringLength(50)]
        public string SKU { get; set; }
        
        public int? ServiceCategoryId { get; set; }
        
        [ForeignKey("ServiceCategoryId")]
        public ServiceCategory? ServiceCategory { get; set; }
        
        public int? DeviceId { get; set; }
        
        [ForeignKey("DeviceId")]
        public Device? Device { get; set; }
    }
}
