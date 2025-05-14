using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepairManagerApi.Data;
using System.Text;

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

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

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

// Initialize and seed the database
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<RepairManagerContext>();
        
        Console.WriteLine("Starting database initialization...");
        
        // Ensure database is created
        Console.WriteLine("Ensuring database is created...");
        context.Database.EnsureCreated();
        
        // Apply migrations to create or update the database schema
        Console.WriteLine("Applying database migrations...");
        context.Database.Migrate();
        Console.WriteLine("Database migrations applied successfully.");
        
        // Check if database has any tables
        Console.WriteLine("Checking database tables...");
        
        // Then seed the database with initial data
        // Temporarily disabled for migration
        /*
        try
        {
            Console.WriteLine("Starting database seeding...");
            DbSeeder.SeedData(context);
            Console.WriteLine("Database seeded successfully.");
        }
        catch (Exception seedEx)
        {
            Console.WriteLine($"ERROR: Database seeding failed: {seedEx.Message}");
            Console.WriteLine(seedEx.StackTrace);
            // Re-throw to ensure the application doesn't start with an improperly seeded database
            throw;
        }
        */
        Console.WriteLine("Database seeding temporarily disabled for migration.");
        
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

app.Run();
