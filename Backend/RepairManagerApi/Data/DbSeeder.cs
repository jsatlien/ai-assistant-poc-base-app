using RepairManagerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace RepairManagerApi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDataAsync(RepairManagerContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("Starting database seeding process...");
            
            // 1. Seed the Group
            int mainGroupId = SeedMainGroup(context);
            Console.WriteLine($"Main Group created with ID: {mainGroupId}");
            
            // 2. Seed Identity roles first
            await SeedIdentityRolesAsync(roleManager);
            Console.WriteLine("Identity roles created successfully");
            
            // 3. Seed the Administrator role (legacy)
            int adminRoleId = SeedAdminRole(context);
            Console.WriteLine($"Administrator role created with ID: {adminRoleId}");
            
            // 4. Seed the admin user
            string adminUserId = await SeedAdminUserAsync(context, userManager, adminRoleId, mainGroupId);
            Console.WriteLine($"Admin user created with ID: {adminUserId}");
            
            // 5. Seed manufacturers
            SeedManufacturers(context);
            Console.WriteLine("Manufacturers seeded successfully");
            
            Console.WriteLine("Database seeding completed successfully!");
        }

        private static int SeedMainGroup(RepairManagerContext context)
        {
            Console.WriteLine("Creating Main Group...");
            
            // Check if the Main group already exists
            var existingGroup = context.Groups.FirstOrDefault(g => g.Code == "MAIN");
            if (existingGroup != null)
            {
                Console.WriteLine($"Main Group already exists with ID: {existingGroup.Id}");
                return existingGroup.Id;
            }
            
            // Create the Main group
            var mainGroup = new Group
            {
                Code = "MAIN",
                Description = "Main Organization Group",
                Address1 = "123 Repair Street",
                Address2 = "Suite 100",
                Address3 = "Building A",
                Address4 = "Floor 2",
                City = "Chicago",
                State = "IL",
                Zip = "60601",
                Country = "USA"
            };
            
            context.Groups.Add(mainGroup);
            context.SaveChanges();
            
            Console.WriteLine($"Main Group created with ID: {mainGroup.Id}");
            return mainGroup.Id;
        }

        private static void SeedManufacturers(RepairManagerContext context)
        {
            Console.WriteLine("Creating manufacturer records...");
            
            // Check if manufacturers already exist
            if (context.Manufacturers.Any())
            {
                Console.WriteLine("Manufacturers already exist in the database.");
                return;
            }
            
            // Create the 5 manufacturer records
            var manufacturers = new List<Manufacturer>
            {
                new Manufacturer { Name = "Apple", Description = "Consumer electronics and software" },
                new Manufacturer { Name = "Google", Description = "Technology and software company" },
                new Manufacturer { Name = "Dell", Description = "Computer hardware manufacturer" },
                new Manufacturer { Name = "Lenovo", Description = "Computer manufacturer" },
                new Manufacturer { Name = "Samsung", Description = "Electronics manufacturer" }
            };

            context.Manufacturers.AddRange(manufacturers);
            context.SaveChanges();
            
            Console.WriteLine($"Added {manufacturers.Count} manufacturer records to the database.");
        }
        
        private static int SeedAdminRole(RepairManagerContext context)
        {
            Console.WriteLine("Creating Administrator role...");
            
            // Check if the Administrator role already exists
            var existingRole = context.UserRoles.FirstOrDefault(r => r.Name == "Administrator");
            if (existingRole != null)
            {
                Console.WriteLine($"Administrator role already exists with ID: {existingRole.Id}");
                return existingRole.Id;
            }
            
            // Create the Administrator role with all privileges
            var adminRole = new UserRole
            {
                Name = "Administrator",
                Description = "System administrator with full access",
                CanManageUsers = true,
                CanManageRoles = true,
                CanManageInventory = true,
                CanManageCatalog = true,
                CanCreateWorkOrders = true,
                CanEditWorkOrders = true,
                CanDeleteWorkOrders = true,
                CanManagePrograms = true,
                CanManageWorkflows = true
            };
            
            context.UserRoles.Add(adminRole);
            context.SaveChanges();
            
            Console.WriteLine($"Administrator role created with ID: {adminRole.Id}");
            return adminRole.Id;
        }
        
        private static async Task SeedIdentityRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("Creating Identity roles...");
            
            // Define the roles we want to create
            string[] roleNames = { "Administrator", "User", "Manager", "Technician" };
            
            foreach (var roleName in roleNames)
            {
                // Check if the role already exists
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    // Create the role
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                    
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"Created role: {roleName}");
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        Console.WriteLine($"Failed to create role {roleName}: {errors}");
                    }
                }
                else
                {
                    Console.WriteLine($"Role {roleName} already exists");
                }
            }
        }
        
        private static async Task<string> SeedAdminUserAsync(RepairManagerContext context, UserManager<User> userManager, int adminRoleId, int groupId)
        {
            Console.WriteLine("Creating admin user...");
            
            // Check if the admin user already exists
            var existingUser = await userManager.FindByNameAsync("admin");
            if (existingUser != null)
            {
                Console.WriteLine($"Admin user already exists with ID: {existingUser.Id}");
                
                // Ensure the user is in the Administrator role
                if (!await userManager.IsInRoleAsync(existingUser, "Administrator"))
                {
                    await userManager.AddToRoleAsync(existingUser, "Administrator");
                    Console.WriteLine("Added existing admin user to Administrator role");
                }
                
                return existingUser.Id;
            }
            
            // Create the admin user with Identity
            var adminUser = new User
            {
                UserName = "admin",
                FullName = "System Administrator",
                Email = "admin@example.com",
                RoleId = adminRoleId,
                GroupId = groupId,
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(adminUser, "admin123");
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create admin user: {errors}");
            }
            
            // Add the admin user to the Administrator role
            await userManager.AddToRoleAsync(adminUser, "Administrator");
            
            Console.WriteLine($"Admin user created with ID: {adminUser.Id}");
            return adminUser.Id;
        }
        
        // Helper method to hash passwords (legacy method, not used with Identity)
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        
        private static void SeedProductCategories(RepairManagerContext context)
        {
            if (!context.ProductCategories.Any())
            {
                var categories = new List<ProductCategory>
                {
                    new ProductCategory { Name = "Laptops", Description = "Portable computers" },
                    new ProductCategory { Name = "Desktops", Description = "Desktop computers" },
                    new ProductCategory { Name = "Tablets", Description = "Tablet devices" },
                    new ProductCategory { Name = "Smartphones", Description = "Mobile phones" },
                    new ProductCategory { Name = "Printers", Description = "Printing devices" }
                };

                context.ProductCategories.AddRange(categories);
                context.SaveChanges();
            }
        }

        private static void SeedServiceCategories(RepairManagerContext context)
        {
            if (!context.ServiceCategories.Any())
            {
                var categories = new List<ServiceCategory>
                {
                    new ServiceCategory { Name = "Hardware Repair", Description = "Physical component repairs" },
                    new ServiceCategory { Name = "Software Support", Description = "Software troubleshooting and fixes" },
                    new ServiceCategory { Name = "Data Recovery", Description = "Recovering lost or damaged data" },
                    new ServiceCategory { Name = "Network Setup", Description = "Setting up and configuring networks" },
                    new ServiceCategory { Name = "Maintenance", Description = "Regular maintenance services" }
                };

                context.ServiceCategories.AddRange(categories);
                context.SaveChanges();
            }
        }
        
        private static void SeedStatusCodes(RepairManagerContext context)
        {
            // Skip seeding status codes for now to avoid errors
            Console.WriteLine("Skipping status code seeding for now to avoid errors.");
            return;
        }
        
        private static void SeedUserRoles(RepairManagerContext context)
        {
            if (!context.UserRoles.Any())
            {
                var roles = new List<UserRole>
                {
                    new UserRole
                    {
                        Name = "Administrator",
                        Description = "Full system access",
                        CanManageUsers = true,
                        CanManageRoles = true,
                        CanManageInventory = true,
                        CanManageCatalog = true,
                        CanCreateWorkOrders = true,
                        CanEditWorkOrders = true,
                        CanDeleteWorkOrders = true,
                        CanManagePrograms = true,
                        CanManageWorkflows = true
                    },
                    new UserRole
                    {
                        Name = "Manager",
                        Description = "Group manager",
                        CanManageUsers = true,
                        CanManageRoles = false,
                        CanManageInventory = true,
                        CanManageCatalog = true,
                        CanCreateWorkOrders = true,
                        CanEditWorkOrders = true,
                        CanDeleteWorkOrders = true,
                        CanManagePrograms = false,
                        CanManageWorkflows = false
                    },
                    new UserRole
                    {
                        Name = "Technician",
                        Description = "Repair technician",
                        CanManageUsers = false,
                        CanManageRoles = false,
                        CanManageInventory = true,
                        CanManageCatalog = false,
                        CanCreateWorkOrders = true,
                        CanEditWorkOrders = true,
                        CanDeleteWorkOrders = false,
                        CanManagePrograms = false,
                        CanManageWorkflows = false
                    },
                    new UserRole
                    {
                        Name = "Front Desk",
                        Description = "Customer service",
                        CanManageUsers = false,
                        CanManageRoles = false,
                        CanManageInventory = false,
                        CanManageCatalog = false,
                        CanCreateWorkOrders = true,
                        CanEditWorkOrders = false,
                        CanDeleteWorkOrders = false,
                        CanManagePrograms = false,
                        CanManageWorkflows = false
                    }
                };

                context.UserRoles.AddRange(roles);
                context.SaveChanges();
            }
        }
        
        private static void SeedUsers(RepairManagerContext context)
        {
            Console.WriteLine("Skipping user seeding for now to avoid errors.");
            return;
        }
        
        private static void SeedDevices(RepairManagerContext context)
        {
            if (!context.Devices.Any())
            {
                var apple = context.Manufacturers.FirstOrDefault(m => m.Name == "Apple");
                var samsung = context.Manufacturers.FirstOrDefault(m => m.Name == "Samsung");
                
                var smartphones = context.ProductCategories.FirstOrDefault(c => c.Name == "Smartphones");
                var laptops = context.ProductCategories.FirstOrDefault(c => c.Name == "Laptops");
                
                // Check if required manufacturers and categories exist
                if (apple == null || samsung == null || smartphones == null || laptops == null)
                {
                    Console.WriteLine("Cannot seed devices: One or more required manufacturers or product categories are missing.");
                    return;
                }
                
                var devices = new List<Device>
                {
                    new Device
                    {
                        Name = "MacBook Pro",
                        Description = "Apple MacBook Pro laptop",
                        SKU = "APPL-MBP-001",
                        ManufacturerId = apple.Id,
                        ProductCategoryId = laptops.Id
                    },
                    new Device
                    {
                        Name = "iPhone 14",
                        Description = "Apple iPhone 14 smartphone",
                        SKU = "APPL-IP14-001",
                        ManufacturerId = apple.Id,
                        ProductCategoryId = smartphones.Id
                    },
                    new Device
                    {
                        Name = "Galaxy S23",
                        Description = "Samsung Galaxy S23 smartphone",
                        SKU = "SAMS-GS23-001",
                        ManufacturerId = samsung.Id,
                        ProductCategoryId = smartphones.Id
                    }
                };

                context.Devices.AddRange(devices);
                context.SaveChanges();
            }
        }
        
        private static void SeedServices(RepairManagerContext context)
        {
            if (!context.Services.Any())
            {
                var hardwareRepair = context.ServiceCategories.FirstOrDefault(c => c.Name == "Hardware Repair");
                var softwareSupport = context.ServiceCategories.FirstOrDefault(c => c.Name == "Software Support");
                
                // Check if required categories exist
                if (hardwareRepair == null || softwareSupport == null)
                {
                    Console.WriteLine("Cannot seed services: One or more required service categories are missing.");
                    return;
                }
                
                var services = new List<Service>
                {
                    new Service
                    {
                        Name = "Screen Replacement",
                        Description = "Replace damaged or broken screens",
                        SKU = "SRV-SCRN-001",
                        ServiceCategoryId = hardwareRepair.Id
                    },
                    new Service
                    {
                        Name = "Battery Replacement",
                        Description = "Replace old or damaged batteries",
                        SKU = "SRV-BATT-001",
                        ServiceCategoryId = hardwareRepair.Id
                    },
                    new Service
                    {
                        Name = "Virus Removal",
                        Description = "Remove viruses and malware",
                        SKU = "SRV-VRUS-001",
                        ServiceCategoryId = softwareSupport.Id
                    }
                };

                context.Services.AddRange(services);
                context.SaveChanges();
            }
        }
        
        private static void SeedParts(RepairManagerContext context)
        {
            if (!context.Parts.Any())
            {
                var apple = context.Manufacturers.FirstOrDefault(m => m.Name == "Apple");
                var samsung = context.Manufacturers.FirstOrDefault(m => m.Name == "Samsung");
                
                // Check if required manufacturers exist
                if (apple == null || samsung == null)
                {
                    Console.WriteLine("Cannot seed parts: One or more required manufacturers are missing.");
                    return;
                }
                
                var parts = new List<Part>
                {
                    new Part
                    {
                        Name = "MacBook Pro Screen",
                        Description = "Replacement screen for MacBook Pro",
                        SKU = "PART-MBP-SCR-001",
                        ManufacturerId = apple.Id
                    },
                    new Part
                    {
                        Name = "iPhone 14 Screen",
                        Description = "Replacement screen for iPhone 14",
                        SKU = "PART-IP14-SCR-001",
                        ManufacturerId = apple.Id
                    },
                    new Part
                    {
                        Name = "Galaxy S23 Screen",
                        Description = "Replacement screen for Galaxy S23",
                        SKU = "PART-GS23-SCR-001",
                        ManufacturerId = samsung.Id
                    }
                };

                context.Parts.AddRange(parts);
                context.SaveChanges();
            }
        }
        
        private static void SeedRepairWorkflows(RepairManagerContext context)
        {
            if (!context.RepairWorkflows.Any())
            {
                var repairWorkflows = new List<RepairWorkflow>
                {
                    new RepairWorkflow
                    {
                        Name = "Standard Repair Workflow",
                        StatusesJson = System.Text.Json.JsonSerializer.Serialize(new List<string> { "PENDING", "DIAGNOSING", "REPAIRING", "TESTING", "COMPLETED" })
                    },
                    new RepairWorkflow
                    {
                        Name = "Premium Repair Workflow",
                        StatusesJson = System.Text.Json.JsonSerializer.Serialize(new List<string> { "PRIORITY", "DIAGNOSING", "PARTS_ORDERED", "REPAIRING", "TESTING", "COMPLETED" })
                    },
                    new RepairWorkflow
                    {
                        Name = "Warranty Repair Workflow",
                        StatusesJson = System.Text.Json.JsonSerializer.Serialize(new List<string> { "WARRANTY_VERIFICATION", "DIAGNOSING", "APPROVAL", "REPAIRING", "TESTING", "COMPLETED" })
                    },
                    new RepairWorkflow
                    {
                        Name = "Business Repair Workflow",
                        StatusesJson = System.Text.Json.JsonSerializer.Serialize(new List<string> { "BUSINESS_INTAKE", "DIAGNOSING", "QUOTE_APPROVAL", "REPAIRING", "TESTING", "COMPLETED" })
                    }
                };

                context.RepairWorkflows.AddRange(repairWorkflows);
                context.SaveChanges();
            }
        }
        
        private static void SeedRepairPrograms(RepairManagerContext context)
        {
            if (!context.RepairPrograms.Any())
            {
                var standardWorkflow = context.RepairWorkflows.FirstOrDefault(w => w.Name == "Standard Repair Workflow");
                var premiumWorkflow = context.RepairWorkflows.FirstOrDefault(w => w.Name == "Premium Repair Workflow");
                var warrantyWorkflow = context.RepairWorkflows.FirstOrDefault(w => w.Name == "Warranty Repair Workflow");
                var businessWorkflow = context.RepairWorkflows.FirstOrDefault(w => w.Name == "Business Repair Workflow");
                
                // Check if required workflows exist
                if (standardWorkflow == null || premiumWorkflow == null || 
                    warrantyWorkflow == null || businessWorkflow == null)
                {
                    Console.WriteLine("Cannot seed repair programs: One or more required repair workflows are missing.");
                    return;
                }
                
                var repairPrograms = new List<RepairProgram>
                {
                    new RepairProgram
                    {
                        Name = "Standard Repair",
                        RepairWorkflowId = standardWorkflow.Id
                    },
                    new RepairProgram
                    {
                        Name = "Premium Repair",
                        RepairWorkflowId = premiumWorkflow.Id
                    },
                    new RepairProgram
                    {
                        Name = "Warranty Repair",
                        RepairWorkflowId = warrantyWorkflow.Id
                    },
                    new RepairProgram
                    {
                        Name = "Business Repair",
                        RepairWorkflowId = businessWorkflow.Id
                    }
                };

                context.RepairPrograms.AddRange(repairPrograms);
                context.SaveChanges();
            }
        }
        
        private static void SeedWorkOrders(RepairManagerContext context)
        {
            if (!context.WorkOrders.Any())
            {
                var hqGroup = context.Groups.FirstOrDefault(g => g.Code == "HQ");
                var macbookPro = context.Devices.FirstOrDefault(d => d.Name == "MacBook Pro");
                var screenReplacement = context.Services.FirstOrDefault(s => s.Name == "Screen Replacement");
                var standardRepair = context.RepairPrograms.FirstOrDefault(p => p.Name == "Standard Repair");
                
                // Check if all required entities exist
                if (hqGroup == null || macbookPro == null || screenReplacement == null || standardRepair == null)
                {
                    Console.WriteLine("Cannot seed work orders: One or more required entities are missing.");
                    return;
                }
                
                var workOrders = new List<WorkOrder>
                {
                    new WorkOrder
                    {
                        Code = "WO00001",
                        DeviceId = macbookPro.Id,
                        ServiceId = screenReplacement.Id,
                        RepairProgramId = standardRepair.Id,
                        GroupId = hqGroup.Id,
                        CustomerName = "John Smith",
                        CustomerPhone = "555-123-4567",
                        IssueDescription = "Cracked screen on MacBook Pro",
                        CurrentStatus = "PENDING",
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        DeviceName = macbookPro.Name,
                        ServiceName = screenReplacement.Name
                    }
                };

                context.WorkOrders.AddRange(workOrders);
                context.SaveChanges();
            }
        }
    }
}
