using BookingSystem.Web.Data;
using BookingSystem.Web.Models;
using BookingSystem.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetupController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher;

    public SetupController(AppDbContext context, PasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpGet("status")]
    public ActionResult GetStatus()
    {
        return Ok(new
        {
            admins = _context.Admins.Count(),
            residents = _context.Residents.Count(),
            facilities = _context.Facilities.Count(),
            timeSlots = _context.TimeSlots.Count()
        });
    }

    [HttpPost("reset")]
    public async Task<ActionResult> ResetAccounts()
    {
        // Delete existing accounts
        _context.Admins.RemoveRange(_context.Admins);
        _context.Residents.RemoveRange(_context.Residents);
        await _context.SaveChangesAsync();

        // Create admin
        _context.Admins.Add(new Admin
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@bookingsystem.fi",
            PasswordHash = _passwordHasher.HashPassword("Admin@123"),
            TwoFactorEnabled = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // Create residents
        _context.Residents.Add(new Resident
        {
            Id = Guid.NewGuid(),
            Name = "Matti Virtanen",
            Email = "matti.virtanen@example.com",
            Phone = "+358401234567",
            ApartmentNumber = "A101",
            PasswordHash = _passwordHasher.HashPassword("Matti@123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        _context.Residents.Add(new Resident
        {
            Id = Guid.NewGuid(),
            Name = "Liisa Korhonen",
            Email = "liisa.korhonen@example.com",
            Phone = "+358407654321",
            ApartmentNumber = "B205",
            PasswordHash = _passwordHasher.HashPassword("Liisa@123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Accounts reset successfully",
            accounts = new
            {
                admin = "admin / Admin@123",
                resident1 = "A101 / Matti@123",
                resident2 = "B205 / Liisa@123"
            }
        });
    }
}
