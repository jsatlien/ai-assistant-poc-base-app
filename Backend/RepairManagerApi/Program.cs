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
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:8080", 
                "http://localhost:3000",
                "http://localhost:5173", // Vite default
                "http://localhost:5174",
                "http://127.0.0.1:5173",
                "http://127.0.0.1:8080")
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
        
        // Apply migrations to create or update the database schema
        context.Database.Migrate();
        Console.WriteLine("Database migrations applied successfully.");
        
        // Then seed the database with initial data
        try
        {
            DbSeeder.SeedData(context);
            Console.WriteLine("Database seeded successfully.");
        }
        catch (Exception seedEx)
        {
            Console.WriteLine($"Warning: Error during database seeding: {seedEx.Message}");
            Console.WriteLine(seedEx.StackTrace);
            // Continue execution even if seeding fails
        }
        
        Console.WriteLine("Database initialization completed.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Critical error while initializing the database: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

// Commented out for development to allow HTTP connections
// app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowVueApp");

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
