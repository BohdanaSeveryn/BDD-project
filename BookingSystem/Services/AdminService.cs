using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Models;
using BookingSystem.Repositories;

namespace BookingSystem.Services;

/// <summary>
/// Service for admin account management and operations
/// Handles admin authentication, resident management, and booking operations
/// </summary>
public class AdminService : IAdminService
{
    private readonly DataStore _dataStore;

    public AdminService(DataStore dataStore)
    {
        _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
    }

    public async Task<bool> IsAdminAuthorizedAsync(Guid adminId)
    {
        await Task.Delay(5); // Simulate async operation

        var admin = _dataStore.Admins.FirstOrDefault(a => a.Id == adminId && a.IsActive);
        return admin != null;
    }

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        await Task.Delay(10); // Simulate async operation

        var admin = _dataStore.Admins.FirstOrDefault(a => a.Username == username);
        if (admin == null || !admin.IsActive)
            return false;

        return DataStore.VerifyPassword(password, admin.PasswordHash);
    }

    public async Task<ResidentAccount> CreateResidentAsync(CreateResidentRequest request)
    {
        await Task.Delay(10); // Simulate async operation

        var resident = new ResidentAccount
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ApartmentNumber = request.ApartmentNumber,
            Email = request.Email,
            Phone = request.Phone,
            IsActive = false,
            ActivationToken = GenerateActivationToken(),
            ActivationTokenExpiry = DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow
        };

        _dataStore.Residents.Add(resident);
        return resident;
    }

    public async Task<bool> DeleteResidentAccountAsync(Guid residentId)
    {
        await Task.Delay(10); // Simulate async operation

        var resident = _dataStore.Residents.FirstOrDefault(r => r.Id == residentId);
        if (resident == null)
            return false;

        // Cancel all associated bookings
        var bookings = _dataStore.Bookings.Where(b => b.ResidentId == residentId).ToList();
        foreach (var booking in bookings)
        {
            _dataStore.Bookings.Remove(booking);
        }

        _dataStore.Residents.Remove(resident);
        return true;
    }

    public async Task<bool> UpdateResidentAsync(Guid residentId, UpdateResidentRequest request)
    {
        await Task.Delay(10); // Simulate async operation

        var resident = _dataStore.Residents.FirstOrDefault(r => r.Id == residentId);
        if (resident == null)
            return false;

        resident.Name = request.Name;
        resident.Email = request.Email;
        resident.Phone = request.Phone;
        resident.ApartmentNumber = request.ApartmentNumber;
        resident.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public async Task<List<BookingCalendarItem>> GetAllBookingsAsync(DateTime bookingDate)
    {
        await Task.Delay(10); // Simulate async operation

        var facility = _dataStore.Facilities.FirstOrDefault();
        if (facility == null)
            return new List<BookingCalendarItem>();

        var timeSlots = _dataStore.TimeSlots
            .Where(ts => ts.Date.Date == bookingDate.Date && ts.FacilityId == facility.Id)
            .OrderBy(ts => ts.StartTime)
            .ToList();

        return timeSlots.Select(ts => new BookingCalendarItem
        {
            Id = ts.Id,
            StartTime = ts.StartTime,
            EndTime = ts.EndTime,
            IsBooked = !ts.IsAvailable,
            ResidentName = GetResidentNameForTimeSlot(ts.Id),
            DataError = null
        }).ToList();
    }

    public async Task<List<BookingCalendarItem>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        await Task.Delay(10); // Simulate async operation

        var facility = _dataStore.Facilities.FirstOrDefault();
        if (facility == null)
            return new List<BookingCalendarItem>();

        var timeSlots = _dataStore.TimeSlots
            .Where(ts => ts.Date >= startDate && ts.Date <= endDate && ts.FacilityId == facility.Id)
            .OrderBy(ts => ts.Date)
            .ThenBy(ts => ts.StartTime)
            .ToList();

        return timeSlots.Select(ts => new BookingCalendarItem
        {
            Id = ts.Id,
            StartTime = ts.StartTime,
            EndTime = ts.EndTime,
            IsBooked = !ts.IsAvailable,
            ResidentName = GetResidentNameForTimeSlot(ts.Id),
            DataError = null
        }).ToList();
    }

    public async Task<AdminBookingDetails> GetBookingDetailsAsync(Guid bookingId)
    {
        await Task.Delay(10); // Simulate async operation

        var booking = _dataStore.Bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking == null)
        {
            return new AdminBookingDetails
            {
                Id = bookingId,
                DataIncomplete = true,
                ErrorMessage = "Booking not found"
            };
        }

        var resident = _dataStore.Residents.FirstOrDefault(r => r.Id == booking.ResidentId);
        if (resident == null)
        {
            return new AdminBookingDetails
            {
                Id = bookingId,
                DataIncomplete = true,
                ErrorMessage = "Resident data missing"
            };
        }

        return new AdminBookingDetails
        {
            Id = booking.Id,
            ResidentName = resident.Name,
            ApartmentNumber = resident.ApartmentNumber,
            Email = resident.Email,
            Phone = resident.Phone,
            Date = booking.Date,
            Time = booking.Time,
            DataIncomplete = false
        };
    }

    public async Task<bool> CancelBookingAsync(AdminCancellationRequest request)
    {
        await Task.Delay(10); // Simulate async operation

        var booking = _dataStore.Bookings.FirstOrDefault(b => b.Id == request.BookingId);
        if (booking == null)
            return false;

        var timeSlot = _dataStore.TimeSlots.FirstOrDefault(ts => ts.Id == booking.TimeSlotId);
        if (timeSlot != null)
        {
            timeSlot.IsAvailable = true;
        }

        _dataStore.Bookings.Remove(booking);
        return true;
    }

    private string? GetResidentNameForTimeSlot(Guid timeSlotId)
    {
        var booking = _dataStore.Bookings.FirstOrDefault(b => b.TimeSlotId == timeSlotId);
        if (booking == null)
            return null;

        var resident = _dataStore.Residents.FirstOrDefault(r => r.Id == booking.ResidentId);
        return resident?.Name;
    }

    public async Task<ResidentAccount?> GetResidentByIdAsync(Guid residentId)
    {
        await Task.Delay(5); // Simulate async operation

        return _dataStore.Residents.FirstOrDefault(r => r.Id == residentId);
    }

    private static string GenerateActivationToken()
    {
        return Guid.NewGuid().ToString("N");
    }
}
