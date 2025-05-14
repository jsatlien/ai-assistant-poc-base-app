using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepairManagerApi.Data;
using System;

namespace RepairManagerSeeder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== RepairManager Database Seeder ===\n");
            Console.WriteLine("This tool will seed the database with initial data.");
            Console.WriteLine("The following will be created:");
            Console.WriteLine("  - A 'Main' Group");
            Console.WriteLine("  - An 'Administrator' user role with all privileges");
            Console.WriteLine("  - An 'admin' user with password 'admin123'");
            Console.WriteLine("  - Five manufacturer records (Apple, Google, Dell, Lenovo, Samsung)\n");
            
            Console.Write("Do you want to proceed? (y/n): ");
            var response = Console.ReadLine()?.ToLower();
            
            if (response != "y" && response != "yes")
            {
                Console.WriteLine("Operation cancelled.");
                return;
            }
            
            try
            {
                // Setup the database context
                var services = new ServiceCollection();
                
                // Configure the database context to use SQLite
                services.AddDbContext<RepairManagerContext>(options =>
                    options.UseSqlite("Data Source=../RepairManagerApi/RepairManager.db"));
                
                var serviceProvider = services.BuildServiceProvider();
                
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<RepairManagerContext>();
                    
                    // Ensure database exists
                    dbContext.Database.EnsureCreated();
                    
                    Console.WriteLine("\nConnected to database. Starting seeding process...\n");
                    
                    // Call the DbSeeder to seed the database
                    DbSeeder.SeedData(dbContext);
                    
                    Console.WriteLine("\nDatabase seeding completed successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
