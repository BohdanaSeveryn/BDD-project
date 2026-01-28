using BookingSystem.Web.Data;
using BookingSystem.Web.DTOs;
using BookingSystem.Web.Models;
using BookingSystem.Web.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Web.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher;

    public AdminService(AppDbContext context, PasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        var admin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Username == username);

        if (admin == null || !admin.IsActive)
            return false;

        return _passwordHasher.VerifyPassword(password, admin.PasswordHash);
    }

    public async Task<List<AdminBookingDetails>> GetAllBookingsAsync(DateTime bookingDate)
    {
        var bookings = await _context.Bookings
            .Where(b => b.BookingDate == bookingDate.Date && b.Status == "Confirmed")
            .Include(b => b.Resident)
            .Include(b => b.Facility)
            .OrderBy(b => b.StartTime)
            .ToListAsync();

        return bookings.Select(b => new AdminBookingDetails
        {
            Id = b.Id,
            BookingDate = b.BookingDate,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            FacilityName = b.Facility.Name,
            ResidentName = b.Resident.Name,
            ResidentApartment = b.Resident.ApartmentNumber,
            ResidentEmail = b.Resident.Email,
            Status = b.Status
        }).ToList();
    }

    public async Task<AdminBookingDetails?> GetBookingDetailsAsync(Guid bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Resident)
            .Include(b => b.Facility)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            return null;

        return new AdminBookingDetails
        {
            Id = booking.Id,
            BookingDate = booking.BookingDate,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            FacilityName = booking.Facility.Name,
            ResidentName = booking.Resident.Name,
            ResidentApartment = booking.Resident.ApartmentNumber,
            ResidentEmail = booking.Resident.Email,
            Status = booking.Status
        };
    }

    public async Task<bool> DeleteResidentAsync(Guid residentId)
    {
        var resident = await _context.Residents.FindAsync(residentId);
        if (resident == null)
            return false;

        resident.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateResidentAsync(Guid residentId, UpdateResidentRequest request)
    {
        var resident = await _context.Residents.FindAsync(residentId);
        if (resident == null)
            return false;

        resident.Name = request.Name;
        resident.Email = request.Email;
        resident.Phone = request.Phone;
        resident.ApartmentNumber = request.ApartmentNumber;
        resident.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
