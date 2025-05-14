using Microsoft.EntityFrameworkCore;
using RepairManagerApi.Models;
using System.Collections.Generic;

namespace RepairManagerApi.Data
{
    public class RepairManagerContext : DbContext
    {
        public RepairManagerContext(DbContextOptions<RepairManagerContext> options)
            : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<RepairWorkflow> RepairWorkflows { get; set; }
        public DbSet<RepairProgram> RepairPrograms { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<CatalogPricing> CatalogPricing { get; set; }
        public DbSet<StatusCode> StatusCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and seed data
            
            // Seed some initial data for demonstration
            modelBuilder.Entity<Device>().HasData(
                new Device { Id = 1, Name = "iPhone 13", Description = "Apple iPhone 13 smartphone" },
                new Device { Id = 2, Name = "Samsung Galaxy S21", Description = "Samsung Galaxy S21 smartphone" },
                new Device { Id = 3, Name = "MacBook Pro", Description = "Apple MacBook Pro laptop" }
            );

            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Screen Replacement", Description = "Replace damaged screen with a new one" },
                new Service { Id = 2, Name = "Battery Replacement", Description = "Replace old or damaged battery" },
                new Service { Id = 3, Name = "Water Damage Repair", Description = "Repair water damage to device" }
            );

            modelBuilder.Entity<Part>().HasData(
                new Part { Id = 1, Name = "iPhone 13 Screen", SKU = "SCR-IP13" },
                new Part { Id = 2, Name = "Samsung Galaxy S21 Screen", SKU = "SCR-SGS21" },
                new Part { Id = 3, Name = "iPhone Battery", SKU = "BAT-IP" },
                new Part { Id = 4, Name = "Samsung Battery", SKU = "BAT-SG" }
            );

            modelBuilder.Entity<RepairWorkflow>().HasData(
                new RepairWorkflow 
                { 
                    Id = 1, 
                    Name = "Standard Repair Workflow", 
                    StatusesJson = System.Text.Json.JsonSerializer.Serialize(new List<string> 
                    { 
                        "Pending", 
                        "In Progress", 
                        "Quality Check", 
                        "Completed" 
                    })
                },
                new RepairWorkflow 
                { 
                    Id = 2, 
                    Name = "Express Repair Workflow", 
                    StatusesJson = System.Text.Json.JsonSerializer.Serialize(new List<string> 
                    { 
                        "Pending", 
                        "In Progress", 
                        "Completed" 
                    })
                }
            );

            modelBuilder.Entity<RepairProgram>().HasData(
                new RepairProgram { Id = 1, Name = "Standard Phone Repair", RepairWorkflowId = 1 },
                new RepairProgram { Id = 2, Name = "Express Phone Repair", RepairWorkflowId = 2 },
                new RepairProgram { Id = 3, Name = "Premium Laptop Repair", RepairWorkflowId = 1 }
            );

            // Sample work orders
            modelBuilder.Entity<WorkOrder>().HasData(
                new WorkOrder 
                { 
                    Id = 1, 
                    DeviceId = 1, 
                    ServiceId = 1, 
                    RepairProgramId = 1, 
                    CurrentStatus = "In Progress",
                    PartIdsJson = System.Text.Json.JsonSerializer.Serialize(new List<int> { 1 }),
                    CreatedAt = System.DateTime.UtcNow.AddDays(-2)
                },
                new WorkOrder 
                { 
                    Id = 2, 
                    DeviceId = 2, 
                    ServiceId = 2, 
                    RepairProgramId = 2, 
                    CurrentStatus = "Pending",
                    PartIdsJson = System.Text.Json.JsonSerializer.Serialize(new List<int> { 4 }),
                    CreatedAt = System.DateTime.UtcNow.AddDays(-1)
                }
            );
        }
    }
}
