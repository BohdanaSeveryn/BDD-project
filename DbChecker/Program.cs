using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BookingSystem.Web.Data;
using BookingSystem.Web.Utilities;
using BookingSystem.Web.Models;

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlite("Data Source=../BookingSystem.Web/BookingSystem.db");

using var db = new AppDbContext(optionsBuilder.Options);
var passwordHasher = new PasswordHasher();

Console.WriteLine("=== DATABASE STATUS ===");
Console.WriteLine($"Admins: {db.Admins.Count()}");
Console.WriteLine($"Residents: {db.Residents.Count()}");
Console.WriteLine($"Facilities: {db.Facilities.Count()}");

if (!db.Admins.Any())
{
    Console.WriteLine("\nCreating admin...");
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
    Console.WriteLine("✓ Admin created: admin / Admin@123");
}

if (!db.Residents.Any())
{
    Console.WriteLine("\nCreating residents...");
    db.Residents.AddRange(
        new Resident
        {
            Id = Guid.NewGuid(),
            Name = "Matti Virtanen",
            Email = "matti.virtanen@example.com",
            Phone = "+358401234567",
            ApartmentNumber = "A101",
            PasswordHash = passwordHasher.HashPassword("Matti@123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        },
        new Resident
        {
            Id = Guid.NewGuid(),
            Name = "Liisa Korhonen",
            Email = "liisa.korhonen@example.com",
            Phone = "+358407654321",
            ApartmentNumber = "B205",
            PasswordHash = passwordHasher.HashPassword("Liisa@123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        }
    );
    db.SaveChanges();
    Console.WriteLine("✓ Resident 1: A101 / Matti@123");
    Console.WriteLine("✓ Resident 2: B205 / Liisa@123");
}

if (!db.Facilities.Any())
{
    Console.WriteLine("\nCreating facility and time slots...");
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
    
    var startDate = DateTime.Today;
    for (int day = 0; day < 7; day++)
    {
        for (int hour = 8; hour < 22; hour += 2)
        {
            db.TimeSlots.Add(new TimeSlot
            {
                Id = Guid.NewGuid(),
                FacilityId = saunaId,
                Date = startDate.AddDays(day),
                StartTime = new TimeOnly(hour, 0),
                EndTime = new TimeOnly(hour + 2, 0),
                Status = "Available",
                Color = "green",
                CreatedAt = DateTime.UtcNow
            });
        }
    }
    db.SaveChanges();
    Console.WriteLine($"✓ Facility created with {db.TimeSlots.Count()} time slots");
}

Console.WriteLine("\n=== ACCOUNTS READY ===");
Console.WriteLine("Admin: admin / Admin@123");
Console.WriteLine("Resident 1: A101 / Matti@123");
Console.WriteLine("Resident 2: B205 / Liisa@123");
