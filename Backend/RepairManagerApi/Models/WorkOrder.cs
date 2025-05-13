using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RepairManagerApi.Models
{
    public class WorkOrder
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int DeviceId { get; set; }
        
        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
        
        [Required]
        public int ServiceId { get; set; }
        
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }
        
        [Required]
        public int RepairProgramId { get; set; }
        
        [ForeignKey("RepairProgramId")]
        public RepairProgram RepairProgram { get; set; }
        
        [Required]
        [StringLength(50)]
        public string CurrentStatus { get; set; } = "Pending";
        
        // Store part IDs as a JSON array in the database
        public string PartIdsJson { get; set; } = "[]";
        
        // Non-mapped property for easier access in code
        [NotMapped]
        public List<int> PartIds 
        { 
            get => string.IsNullOrEmpty(PartIdsJson) 
                ? new List<int>() 
                : System.Text.Json.JsonSerializer.Deserialize<List<int>>(PartIdsJson);
            set => PartIdsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
