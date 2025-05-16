using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepairManagerApi.Models;
using System.Collections.Generic;

namespace RepairManagerApi.Data
{
    public class RepairManagerContext : IdentityDbContext<User>
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
        // Additional user-related tables
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<CatalogPricing> CatalogPricing { get; set; }
        public DbSet<StatusCode> StatusCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call base to set up Identity tables
            base.OnModelCreating(modelBuilder);
            
            // Configure Identity tables with custom schema
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
            
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                // Configure properties
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Configure relationships
                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId);
                    
                entity.HasOne(e => e.Group)
                    .WithMany()
                    .HasForeignKey(e => e.GroupId);
            });
            
            // Configure relationships for legacy tables
            // Will be handled by DbSeeder.cs for any seeding needs
        }
    }
}
