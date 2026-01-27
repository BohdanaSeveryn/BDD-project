using System;
using System.Collections.Generic;
using BookingSystem.Models;

namespace BookingSystem.Repositories;

/// <summary>
/// In-memory data storage for testing purposes
/// </summary>
public class DataStore
{
    public List<ResidentAccount> Residents { get; } = new();
    public List<AdminAccount> Admins { get; } = new();
    public List<Booking> Bookings { get; } = new();
    public List<TimeSlot> TimeSlots { get; } = new();
    public List<Facility> Facilities { get; } = new();
    public List<TwoFactorCode> TwoFactorCodes { get; } = new();

    public DataStore()
    {
        InitializeDefaults();
    }

    private void InitializeDefaults()
    {
        // Add default facilities
        Facilities.Add(new Facility 
        { 
            Id = Guid.NewGuid(), 
            Name = "Washing Room", 
            Description = "Common laundry facility" 
        });

        // Add default admin account
        Admins.Add(new AdminAccount 
        { 
            Id = Guid.NewGuid(), 
            Username = "admin", 
            PasswordHash = HashPassword("admin123"), 
            IsActive = true,
            IsTwoFactorEnabled = true 
        });
    }

    public static string HashPassword(string password)
    {
        // Simple hash for demo purposes - in production use proper hashing like bcrypt
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    public static bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        return hashOfInput == hash;
    }
}
