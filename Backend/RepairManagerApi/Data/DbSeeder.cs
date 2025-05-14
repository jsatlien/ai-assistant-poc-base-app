using RepairManagerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace RepairManagerApi.Data
{
    public static class DbSeeder
    {
        public static void SeedData(RepairManagerContext context)
        {
            // Seed each entity type individually in the correct order based on foreign key dependencies
            // 1. First seed entities with no foreign key dependencies
            try { SeedUserRoles(context); } catch (Exception ex) { Console.WriteLine($"Error seeding UserRoles: {ex.Message}"); }
            try { SeedGroups(context); } catch (Exception ex) { Console.WriteLine($"Error seeding Groups: {ex.Message}"); }
            try { SeedServiceCategories(context); } catch (Exception ex) { Console.WriteLine($"Error seeding ServiceCategories: {ex.Message}"); }
            try { SeedProductCategories(context); } catch (Exception ex) { Console.WriteLine($"Error seeding ProductCategories: {ex.Message}"); }
            try { SeedManufacturers(context); } catch (Exception ex) { Console.WriteLine($"Error seeding Manufacturers: {ex.Message}"); }
            try { SeedStatusCodes(context); } catch (Exception ex) { Console.WriteLine($"Error seeding StatusCodes: {ex.Message}"); }
            
            // 2. Then seed entities that depend on the above
            try { SeedUsers(context); } catch (Exception ex) { Console.WriteLine($"Error seeding Users: {ex.Message}"); }
            try { SeedDevices(context); } catch (Exception ex) { Console.WriteLine($"Error seeding Devices: {ex.Message}"); }
            
            // 3. Then seed entities that depend on the above
            try { SeedParts(context); } catch (Exception ex) { Console.WriteLine($"Error seeding Parts: {ex.Message}"); }
            try { SeedServices(context); } catch (Exception ex) { Console.WriteLine($"Error seeding Services: {ex.Message}"); }
            try { SeedRepairWorkflows(context); } catch (Exception ex) { Console.WriteLine($"Error seeding RepairWorkflows: {ex.Message}"); }
            
            // 4. Finally seed entities that depend on all the above
            try { SeedRepairPrograms(context); } catch (Exception ex) { Console.WriteLine($"Error seeding RepairPrograms: {ex.Message}"); }
            try { SeedWorkOrders(context); } catch (Exception ex) { Console.WriteLine($"Error seeding WorkOrders: {ex.Message}"); }
        }

        private static void SeedGroups(RepairManagerContext context)
        {
            Console.WriteLine("Starting to seed Groups table...");
            
            try
            {
                // First check if there are any groups
                var groupCount = context.Groups.Count();
                Console.WriteLine($"Found {groupCount} existing groups in the database.");
                
                // If there are groups, remove them to ensure clean seeding
                if (groupCount > 0)
                {
                    Console.WriteLine("Removing existing groups...");
                    foreach (var group in context.Groups.ToList())
                    {
                        context.Groups.Remove(group);
                    }
                    context.SaveChanges();
                    Console.WriteLine("Existing groups removed successfully.");
                }
                
                // Now add the new groups
                Console.WriteLine("Adding new groups...");
            {
                var groups = new List<Group>
                {
                    new Group
                    {
                        Code = "HQ",
                        Description = "Headquarters",
                        Address1 = "123 Main Street",
                        City = "New York",
                        State = "NY",
                        Zip = "10001",
                        Country = "USA"
                    },
                    new Group
                    {
                        Code = "WEST",
                        Description = "West Coast Office",
                        Address1 = "456 Tech Blvd",
                        City = "San Francisco",
                        State = "CA",
                        Zip = "94107",
                        Country = "USA"
                    },
                    new Group
                    {
                        Code = "SOUTH",
                        Description = "South Region Office",
                        Address1 = "789 Palm Drive",
                        City = "Miami",
                        State = "FL",
                        Zip = "33101",
                        Country = "USA"
                    }
                };

                context.Groups.AddRange(groups);
                
                // Save changes and verify the groups were added
                var saveResult = context.SaveChanges();
                Console.WriteLine($"Saved {saveResult} groups to the database.");
                
                // Verify the groups were added by counting them
                var newGroupCount = context.Groups.Count();
                Console.WriteLine($"After seeding, database now contains {newGroupCount} groups.");
                
                if (newGroupCount == 0)
                {
                    throw new Exception("Failed to seed groups. No groups were added to the database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in SeedGroups: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw; // Re-throw to ensure the error is properly handled
            }
        }

        private static void SeedManufacturers(RepairManagerContext context)
        {
            if (!context.Manufacturers.Any())
            {
                var manufacturers = new List<Manufacturer>
                {
                    new Manufacturer { Name = "Apple", Description = "Consumer electronics and software" },
                    new Manufacturer { Name = "Samsung", Description = "Electronics manufacturer" },
                    new Manufacturer { Name = "Dell", Description = "Computer hardware manufacturer" },
                    new Manufacturer { Name = "HP", Description = "Computer and printer manufacturer" },
                    new Manufacturer { Name = "Lenovo", Description = "Computer manufacturer" }
                };

                context.Manufacturers.AddRange(manufacturers);
                context.SaveChanges();
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
