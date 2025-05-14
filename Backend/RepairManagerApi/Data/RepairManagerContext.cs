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
            // Configure relationships
            // Seeding removed to allow migrations to work properly
            // Will be handled by DbSeeder.cs instead
        }
    }
}
