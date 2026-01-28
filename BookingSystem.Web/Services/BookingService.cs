using BookingSystem.Web.Data;
using BookingSystem.Web.DTOs;
using BookingSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Web.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public BookingService(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<List<TimeSlotResponse>> GetAvailabilityAsync(Guid facilityId, DateTime bookingDate)
    {
        var slots = await _context.TimeSlots
            .Where(t => t.FacilityId == facilityId && t.Date == bookingDate.Date)
            .OrderBy(t => t.StartTime)
            .ToListAsync();

        return slots.Select(s => new TimeSlotResponse
        {
            Id = s.Id,
            Date = s.Date,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Status = s.Status,
            Color = s.Color
        }).ToList();
    }

    public async Task<BookingResponse?> ConfirmBookingAsync(CreateBookingRequest request)
    {
        var timeSlot = await _context.TimeSlots.FindAsync(request.TimeSlotId);
        if (timeSlot == null || timeSlot.Status != "Available")
            return null;

        var existingBooking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.TimeSlotId == request.TimeSlotId);
        if (existingBooking != null)
            return null;

        var resident = await _context.Residents.FindAsync(request.ResidentId);
        if (resident == null)
            return null;

        var facility = await _context.Facilities.FindAsync(request.FacilityId);
        if (facility == null)
            return null;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            ResidentId = request.ResidentId,
            TimeSlotId = request.TimeSlotId,
            FacilityId = request.FacilityId,
            BookingDate = timeSlot.Date,
            StartTime = timeSlot.StartTime,
            EndTime = timeSlot.EndTime,
            Status = "Confirmed"
        };

        timeSlot.Status = "Booked";
        timeSlot.Color = "red";

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        await _emailService.SendBookingConfirmationAsync(new BookingConfirmationEmail
        {
            ResidentEmail = resident.Email,
            ResidentName = resident.Name,
            FacilityName = facility.Name,
            BookingDate = booking.BookingDate,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime
        });

        return new BookingResponse
        {
            Id = booking.Id,
            BookingDate = booking.BookingDate,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            FacilityName = facility.Name,
            Status = booking.Status
        };
    }

    public async Task<List<BookingResponse>> GetUpcomingBookingsAsync(Guid residentId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.ResidentId == residentId && b.Status == "Confirmed" && b.BookingDate >= DateTime.UtcNow.Date)
            .Include(b => b.Facility)
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.StartTime)
            .ToListAsync();

        return bookings.Select(b => new BookingResponse
        {
            Id = b.Id,
            BookingDate = b.BookingDate,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            FacilityName = b.Facility.Name,
            Status = b.Status
        }).ToList();
    }

    public async Task<BookingDetails?> GetBookingDetailsAsync(Guid bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Resident)
            .Include(b => b.Facility)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            return null;

        return new BookingDetails
        {
            Id = booking.Id,
            BookingDate = booking.BookingDate,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            FacilityName = booking.Facility.Name,
            ResidentName = booking.Resident.Name,
            Status = booking.Status
        };
    }

    public async Task<bool> CancelBookingAsync(Guid bookingId, Guid residentId)
    {
        var booking = await _context.Bookings
            .Include(b => b.TimeSlot)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.ResidentId == residentId);

        if (booking == null || booking.Status == "Cancelled")
            return false;

        booking.Status = "Cancelled";
        booking.CancelledBy = "Resident";
        booking.CancelledAt = DateTime.UtcNow;

        booking.TimeSlot.Status = "Available";
        booking.TimeSlot.Color = "green";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelBookingAsAdminAsync(Guid bookingId, string reason)
    {
        var booking = await _context.Bookings
            .Include(b => b.TimeSlot)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null || booking.Status == "Cancelled")
            return false;

        booking.Status = "Cancelled";
        booking.CancellationReason = reason;
        booking.CancelledBy = "Admin";
        booking.CancelledAt = DateTime.UtcNow;

        booking.TimeSlot.Status = "Available";
        booking.TimeSlot.Color = "green";

        await _context.SaveChangesAsync();
        return true;
    }
}
