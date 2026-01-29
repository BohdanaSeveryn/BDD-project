using System;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Web.Data;
using BookingSystem.Web.Utilities;
using BookingSystem.Web.Models;

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlite("Data Source=BookingSystem.Web/BookingSystem.db");

using var db = new AppDbContext(optionsBuilder.Options);

Console.WriteLine("=== DATABASE CHECK ===");
Console.WriteLine($"Admins: {db.Admins.Count()}");
Console.WriteLine($"Residents: {db.Residents.Count()}");
Console.WriteLine($"Facilities: {db.Facilities.Count()}");
Console.WriteLine($"TimeSlots: {db.TimeSlots.Count()}");

if (db.Admins.Any())
{
    var admin = db.Admins.First();
    Console.WriteLine($"\nAdmin: {admin.Username}, Email: {admin.Email}, Active: {admin.IsActive}");
}

if (db.Residents.Any())
{
    Console.WriteLine("\nResidents:");
    foreach (var res in db.Residents.Take(5))
    {
        Console.WriteLine($"  - {res.Name} ({res.ApartmentNumber}), Active: {res.IsActive}");
    }
}

// Now let's recreate accounts if needed
var passwordHasher = new PasswordHasher();

if (!db.Admins.Any())
{
    Console.WriteLine("\n=== CREATING ADMIN ===");
    db.Admins.Add(new Admin
    {
        Id = Guid.NewGuid(),
        Username = "admin",
        Email = "admin@bookingsystem.fi",
        PasswordHash = passwordHasher.HashPassword("Admin@123"),
        TwoFactorEnabled = false,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    });
    db.SaveChanges();
    Console.WriteLine("Admin created!");
}

if (!db.Residents.Any())
{
    Console.WriteLine("\n=== CREATING RESIDENTS ===");
    db.Residents.Add(new Resident
    {
        Id = Guid.NewGuid(),
        Name = "Matti Virtanen",
        Email = "matti.virtanen@example.com",
        Phone = "+358401234567",
        ApartmentNumber = "A101",
        PasswordHash = passwordHasher.HashPassword("Matti@123"),
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    });
    
    db.Residents.Add(new Resident
    {
        Id = Guid.NewGuid(),
        Name = "Liisa Korhonen",
        Email = "liisa.korhonen@example.com",
        Phone = "+358407654321",
        ApartmentNumber = "B205",
        PasswordHash = passwordHasher.HashPassword("Liisa@123"),
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    });
    db.SaveChanges();
    Console.WriteLine("Residents created!");
}

if (!db.Facilities.Any())
{
    Console.WriteLine("\n=== CREATING FACILITIES ===");
    var saunaId = Guid.NewGuid();
    db.Facilities.Add(new Facility
    {
        Id = saunaId,
        Name = "Sauna",
        Description = "Taloyhtiön sauna",
        Icon = "🧖",
        IsAvailable = true,
        CreatedAt = DateTime.UtcNow
    });
    db.SaveChanges();
    
    // Create time slots
    var startDate = DateTime.Today;
    for (int day = 0; day < 7; day++)
    {
        var date = startDate.AddDays(day);
        for (int hour = 8; hour < 22; hour += 2)
        {
            db.TimeSlots.Add(new TimeSlot
            {
                Id = Guid.NewGuid(),
                FacilityId = saunaId,
                Date = date,
                StartTime = new TimeOnly(hour, 0),
                EndTime = new TimeOnly(hour + 2, 0),
                Status = "Available",
                Color = "green",
                CreatedAt = DateTime.UtcNow
            });
        }
    }
    db.SaveChanges();
    Console.WriteLine("Facilities and time slots created!");
}

Console.WriteLine("\n=== FINAL CHECK ===");
Console.WriteLine($"Admins: {db.Admins.Count()}");
Console.WriteLine($"Residents: {db.Residents.Count()}");
Console.WriteLine($"Facilities: {db.Facilities.Count()}");
Console.WriteLine($"TimeSlots: {db.TimeSlots.Count()}");
