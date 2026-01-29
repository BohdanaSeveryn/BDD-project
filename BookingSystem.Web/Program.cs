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

        db.Database.ExecuteSqlRaw("DROP INDEX IF EXISTS IX_TimeSlots_FacilityId_Date;");
        db.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS IX_TimeSlots_FacilityId_Date_StartTime_EndTime ON TimeSlots (FacilityId, Date, StartTime, EndTime);");

        // Seed default admin account if it doesn't exist
        var existingAdmin = db.Admins.FirstOrDefault(a => a.Username == "admin");
        if (existingAdmin == null)
        {
            Console.WriteLine("Creating default admin account...");
            var adminId = Guid.NewGuid();
            var hashedPassword = passwordHasher.HashPassword("Admin@123");

            existingAdmin = new BookingSystem.Web.Models.Admin
            {
                Id = adminId,
                Username = "admin",
                Email = "admin@bookingsystem.fi",
                PasswordHash = hashedPassword,
                TwoFactorEnabled = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Admins.Add(existingAdmin);

            db.SaveChanges();
            Console.WriteLine("Default admin account created successfully.");
            Console.WriteLine("Admin credentials: username=admin, password=Admin@123");
        }
        else
        {
            existingAdmin.PasswordHash = passwordHasher.HashPassword("Admin@123");
            existingAdmin.TwoFactorEnabled = false;
            existingAdmin.IsActive = true;
            existingAdmin.UpdatedAt = DateTime.UtcNow;
            db.SaveChanges();
            Console.WriteLine("Admin account updated: username=admin, password=Admin@123");
        }

        var facilities = db.Facilities.ToList();
        var sauna = facilities.FirstOrDefault(f => f.Name == "Sauna");
        if (sauna == null)
        {
            sauna = new BookingSystem.Web.Models.Facility
            {
                Id = Guid.NewGuid(),
                Name = "Sauna",
                Description = "Taloyhtiön sauna",
                Icon = "🧖",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Facilities.Add(sauna);
            facilities.Add(sauna);
        }
        else
        {
            sauna.Description = "Taloyhtiön sauna";
            sauna.Icon = "🧖";
            sauna.IsAvailable = true;
            sauna.UpdatedAt = DateTime.UtcNow;
        }

        var washer = facilities.FirstOrDefault(f => f.Name == "Pesukone 1" || f.Name == "Pyykinpesukone 1" || f.Name == "Пральна машина 1" || f.Name == "Пральна машина 2" || f.Name == "Kuntosali");
        if (washer == null)
        {
            washer = new BookingSystem.Web.Models.Facility
            {
                Id = Guid.NewGuid(),
                Name = "Pesukone 1",
                Description = "Taloyhtiön pesukone",
                Icon = "🧺",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Facilities.Add(washer);
            facilities.Add(washer);
        }
        else
        {
            washer.Name = "Pesukone 1";
            washer.Description = "Taloyhtiön pesukone";
            washer.Icon = "🧺";
            washer.IsAvailable = true;
            washer.UpdatedAt = DateTime.UtcNow;
        }

        foreach (var facility in facilities)
        {
            if (facility.Id == sauna.Id || facility.Id == washer.Id)
                continue;

            facility.IsAvailable = false;
            facility.UpdatedAt = DateTime.UtcNow;
        }

        db.SaveChanges();
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
