using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairManagerApi.Models
{
    public class CatalogPricing
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string ItemType { get; set; } // "Part", "Device", or "Service"
        
        [Required]
        public int ItemId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountAmount { get; set; }
        
        public int? DiscountPercentage { get; set; }
        
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? ExpirationDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Non-mapped property for calculated price
        [NotMapped]
        public decimal EffectivePrice 
        { 
            get 
            {
                decimal price = BasePrice;
                
                if (DiscountAmount.HasValue)
                {
                    price -= DiscountAmount.Value;
                }
                else if (DiscountPercentage.HasValue)
                {
                    price -= (price * DiscountPercentage.Value / 100);
                }
                
                return Math.Max(0, price);
            }
        }
    }
}
