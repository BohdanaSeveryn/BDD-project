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

    private static readonly TimeOnly DailyStartTime = new(7, 0);
    private static readonly TimeOnly DailyEndTime = new(21, 0);
    private const int SlotDurationHours = 2;

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

    public async Task<GenerateTimeSlotsResponse?> GenerateDailyTimeSlotsAsync(Guid facilityId, DateTime date, TimeOnly startTime, TimeOnly endTime)
    {
        var facility = await _context.Facilities.FindAsync(facilityId);
        if (facility == null)
            return null;

        var bookingDate = date.Date;

        if (startTime < DailyStartTime || endTime > DailyEndTime || startTime >= endTime)
            return null;

        var totalDurationHours = endTime.Hour - startTime.Hour;
        if (totalDurationHours <= 0 || totalDurationHours % SlotDurationHours != 0)
            return null;

        var existingSlots = await _context.TimeSlots
            .Where(t => t.FacilityId == facilityId && t.Date == bookingDate)
            .ToListAsync();

        var createdSlots = new List<TimeSlot>();

        for (var hour = startTime.Hour; hour < endTime.Hour; hour += SlotDurationHours)
        {
            var start = new TimeOnly(hour, 0);
            var end = new TimeOnly(hour + SlotDurationHours, 0);

            var exists = existingSlots.Any(s => s.StartTime == start && s.EndTime == end);
            if (exists)
                continue;

            var slot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                FacilityId = facilityId,
                Date = bookingDate,
                StartTime = start,
                EndTime = end,
                Status = "Available",
                Color = "green",
                CreatedAt = DateTime.UtcNow
            };

            createdSlots.Add(slot);
        }

        if (createdSlots.Count > 0)
        {
            await _context.TimeSlots.AddRangeAsync(createdSlots);
            await _context.SaveChangesAsync();
        }

        var allSlots = existingSlots.Concat(createdSlots)
            .Where(s => s.StartTime >= DailyStartTime && s.EndTime <= DailyEndTime)
            .OrderBy(s => s.StartTime)
            .Select(s => new AdminTimeSlotResponse
            {
                Id = s.Id,
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Status = s.Status,
                Color = s.Color
            })
            .ToList();

        var totalExpectedSlots = (endTime.Hour - startTime.Hour) / SlotDurationHours;

        return new GenerateTimeSlotsResponse
        {
            FacilityId = facilityId,
            Date = bookingDate,
            CreatedCount = createdSlots.Count,
            SkippedCount = Math.Max(0, totalExpectedSlots - createdSlots.Count),
            Slots = allSlots
        };
    }

    public async Task<List<FacilityResponse>> GetFacilitiesAsync()
    {
        var facilities = await _context.Facilities
            .Where(f => f.IsAvailable)
            .OrderBy(f => f.Name)
            .ToListAsync();

        return facilities.Select(f => new FacilityResponse
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            Icon = f.Icon
        }).ToList();
    }
}
