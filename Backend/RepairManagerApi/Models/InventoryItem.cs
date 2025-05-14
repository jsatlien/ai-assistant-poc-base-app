using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairManagerApi.Models
{
    public class InventoryItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int GroupId { get; set; }
        
        [ForeignKey("GroupId")]
        public Group Group { get; set; }
        
        [Required]
        public string CatalogItemType { get; set; } // "Part" or "Device"
        
        [Required]
        public int CatalogItemId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public int MinimumQuantity { get; set; }
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        // Non-mapped properties for UI display
        [NotMapped]
        public string CatalogItemName { get; set; }
    }
}
