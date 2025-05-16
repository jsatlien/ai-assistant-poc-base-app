using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepairManagerApi.Data;
using RepairManagerApi.Models;
using RepairManagerApi.Services;
using System.Text;

// Make the program async to support async database seeding
await Main();

async Task Main()
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    // Add SQLite database context
    builder.Services.AddDbContext<RepairManagerContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add CORS policy
    builder.Services.AddCors(options =>
    {
        // Development CORS policy - allows any origin
        options.AddPolicy("Development", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
        
        // Production CORS policy - restricts to specific origins
        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:8080", 
                    "http://localhost:3000",
                    "http://localhost:5173", // Vite default
                    "http://localhost:5174",
                    "http://127.0.0.1:5173",
                    "http://127.0.0.1:8080",
                    "http://localhost:5097")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // Configure ASP.NET Core Identity
    builder.Services.AddIdentity<User, IdentityRole>(options => {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        
        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 5;
        
        // User settings
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<RepairManagerContext>()
    .AddDefaultTokenProviders();

    // Add JWT Authentication
    builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false; // Set to true in production
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero // Reduce the default 5 min tolerance for token expiration
        };
    });

    // Register the TokenService
    builder.Services.AddScoped<RepairManagerApi.Services.TokenService>();

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Configure JSON serialization options if needed
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Initialize the database and seed data
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<RepairManagerContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            
            Console.WriteLine("Starting database initialization...");
            
            // Ensure database is created
            Console.WriteLine("Ensuring database is created...");
            context.Database.EnsureCreated();
            
            // Apply migrations to create or update the database schema
            Console.WriteLine("Applying database migrations...");
            context.Database.Migrate();
            Console.WriteLine("Database migrations applied successfully.");
            
            // Seed the database
            Console.WriteLine("Starting database seeding...");
            await DbSeeder.SeedDataAsync(context, userManager, roleManager);
            Console.WriteLine("Database seeding completed successfully.");
            
            Console.WriteLine("Database initialization completed successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Critical error while initializing the database: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
    }

    // Commented out for development to allow HTTP connections
    // app.UseHttpsRedirection();

    // Use CORS - different policy based on environment
    if (app.Environment.IsDevelopment())
    {
        app.UseCors("Development"); // Allow any origin in development
        Console.WriteLine("Using Development CORS policy - allowing any origin");
    }
    else
    {
        app.UseCors("Production"); // Restrict origins in production
        Console.WriteLine("Using Production CORS policy - restricted origins");
    }

    // Add authentication middleware
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
}
