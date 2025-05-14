using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string? SKU { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public int ManufacturerId { get; set; }
        
        [ForeignKey("ManufacturerId")]
        public Manufacturer Manufacturer { get; set; }
        
        public int? DeviceId { get; set; }
        
        [ForeignKey("DeviceId")]
        public Device? Device { get; set; }
        
        public int? ProductCategoryId { get; set; }
        
        [ForeignKey("ProductCategoryId")]
        public ProductCategory? ProductCategory { get; set; }
    }
}
