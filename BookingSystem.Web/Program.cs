using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BookingSystem.Web.Data;
using BookingSystem.Web.Models;
using BookingSystem.Web.Services;
using BookingSystem.Web.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Services
builder.Services.AddScoped<IResidentService, ResidentService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add Utilities
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<EmailValidator>();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "your-secret-key-min-32-characters-long");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policyBuilder => policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();
    try
    {
        Console.WriteLine("Migrating database...");
        db.Database.Migrate();
        Console.WriteLine("Database migration completed.");

        // Seed default admin account if it doesn't exist
        if (!db.Admins.Any(a => a.Username == "admin"))
        {
            Console.WriteLine("Creating default admin account...");
            var adminId = Guid.NewGuid();
            var hashedPassword = passwordHasher.HashPassword("Admin@123");

            db.Admins.Add(new BookingSystem.Web.Models.Admin
            {
                Id = adminId,
                Username = "admin",
                Email = "admin@bookingsystem.fi",
                PasswordHash = hashedPassword,
                TwoFactorEnabled = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            db.SaveChanges();
            Console.WriteLine("Default admin account created successfully.");
            Console.WriteLine("Admin credentials: username=admin, password=Admin@123");
        }
        else
        {
            Console.WriteLine("Admin account already exists.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization error: {ex.Message}");
    }
}

// Serve static files (HTML, CSS, JS)
app.UseStaticFiles();

// Use CORS
app.UseCors("AllowLocalhost");

// Use Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Default to index.html for SPA routing
app.MapFallbackToFile("index.html");

try
{
    Console.WriteLine("Starting application...");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal error: {ex}");
    throw;
}
