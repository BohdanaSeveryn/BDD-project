using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Models;
using BookingSystem.Repositories;

namespace BookingSystem.Services;

/// <summary>
/// Service for booking operations
/// Handles availability, booking confirmation, cancellation, and calendar management
/// </summary>
public class BookingService : IBookingService
{
    private readonly DataStore _dataStore;
    private readonly Dictionary<Guid, BookingSession> _userSessions = new();

    public BookingService(DataStore dataStore)
    {
        _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
    }

    public async Task<List<TimeSlot>> GetAvailabilityAsync(Guid facilityId, DateTime bookingDate)
    {
        await Task.Delay(10); // Simulate async operation

        return _dataStore.TimeSlots
            .Where(ts => ts.FacilityId == facilityId && ts.Date.Date == bookingDate.Date)
            .OrderBy(ts => ts.StartTime)
            .ToList();
    }

    public async Task<bool> SelectTimeSlotAsync(Guid timeSlotId)
    {
        await Task.Delay(5); // Simulate async operation

        var timeSlot = _dataStore.TimeSlots.FirstOrDefault(ts => ts.Id == timeSlotId);
        if (timeSlot == null || !timeSlot.IsAvailable)
            return false;

        // Store selection in session
        var sessionId = Guid.NewGuid();
        _userSessions[sessionId] = new BookingSession { SelectedSlotId = timeSlotId };

        return true;
    }

    public async Task<bool> ClearSelectionAsync()
    {
        await Task.Delay(5); // Simulate async operation

        var session = _userSessions.Values.FirstOrDefault();
        if (session != null)
        {
            session.SelectedSlotId = null;
        }

        return true;
    }

    public async Task<Booking> ConfirmBookingAsync(CreateBookingRequest request)
    {
        await Task.Delay(10); // Simulate async operation

        var timeSlot = _dataStore.TimeSlots.FirstOrDefault(ts => ts.Id == request.TimeSlotId);
        if (timeSlot == null || !timeSlot.IsAvailable)
        {
            throw new InvalidOperationException("This slot just reserved by another resident");
        }

        var resident = _dataStore.Residents.FirstOrDefault(r => r.Id == request.ResidentId);
        if (resident == null)
        {
            return null;
        }

        // Mark time slot as unavailable
        timeSlot.IsAvailable = false;

        // Create booking
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            ResidentId = request.ResidentId,
            TimeSlotId = request.TimeSlotId,
            Facility = _dataStore.Facilities.FirstOrDefault()?.Name ?? "Unknown",
            Date = timeSlot.Date,
            Time = $"{timeSlot.StartTime} - {timeSlot.EndTime}",
            StartTime = timeSlot.StartTime,
            EndTime = timeSlot.EndTime,
            DayOfWeek = timeSlot.Date.DayOfWeek.ToString(),
            IsMyBooking = true,
            Status = "Confirmed",
            CreatedAt = DateTime.UtcNow
        };

        _dataStore.Bookings.Add(booking);
        return booking;
    }

    public async Task<List<TimeSlot>> RefreshAvailabilityAsync(Guid facilityId, DateTime bookingDate)
    {
        await Task.Delay(10); // Simulate async operation

        return _dataStore.TimeSlots
            .Where(ts => ts.FacilityId == facilityId && ts.Date.Date == bookingDate.Date)
            .OrderBy(ts => ts.StartTime)
            .ToList();
    }

    public async Task<List<Booking>> GetMyBookingsAsync(Guid residentId, DateTime bookingDate)
    {
        await Task.Delay(10); // Simulate async operation

        return _dataStore.Bookings
            .Where(b => b.ResidentId == residentId && b.Date.Date == bookingDate.Date)
            .OrderBy(b => b.StartTime)
            .ToList();
    }

    public async Task<List<Booking>> GetCalendarBookingsAsync(Guid facilityId, DateTime bookingDate)
    {
        await Task.Delay(10); // Simulate async operation

        var facility = _dataStore.Facilities.FirstOrDefault(f => f.Id == facilityId);
        if (facility == null)
            return new List<Booking>();

        return _dataStore.Bookings
            .Where(b => b.Date.Date == bookingDate.Date && b.Facility == facility.Name)
            .OrderBy(b => b.StartTime)
            .ToList();
    }

    public async Task<BookingDetails> GetBookingDetailsAsync(Guid bookingId)
    {
        await Task.Delay(10); // Simulate async operation

        var booking = _dataStore.Bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking == null)
        {
            return new BookingDetails
            {
                Id = bookingId,
                CanCancelBooking = false
            };
        }

        return new BookingDetails
        {
            Id = booking.Id,
            Facility = booking.Facility,
            Date = booking.Date,
            Time = booking.Time,
            CanManage = true,
            CanCancelBooking = CanCancelBooking(booking)
        };
    }

    public async Task<bool> IsUserAuthorizedAsync(Guid residentId)
    {
        await Task.Delay(5); // Simulate async operation

        var resident = _dataStore.Residents.FirstOrDefault(r => r.Id == residentId && r.IsActive);
        return resident != null;
    }

    public async Task<List<Booking>> GetUpcomingBookingsAsync(Guid residentId)
    {
        await Task.Delay(10); // Simulate async operation

        var today = DateTime.UtcNow.Date;
        return _dataStore.Bookings
            .Where(b => b.ResidentId == residentId && b.Date.Date >= today)
            .OrderBy(b => b.Date)
            .ThenBy(b => b.StartTime)
            .ToList();
    }

    public async Task<bool> CancelBookingAsync(Guid bookingId, Guid residentId)
    {
        await Task.Delay(10); // Simulate async operation

        var booking = _dataStore.Bookings.FirstOrDefault(b => b.Id == bookingId && b.ResidentId == residentId);
        if (booking == null)
            return false;

        if (!CanCancelBooking(booking))
            return false;

        // Free up the time slot
        var timeSlot = _dataStore.TimeSlots.FirstOrDefault(ts => ts.Id == booking.TimeSlotId);
        if (timeSlot != null)
        {
            timeSlot.IsAvailable = true;
        }

        _dataStore.Bookings.Remove(booking);
        return true;
    }

    public async Task<bool> CanCancelBookingAsync(Guid bookingId)
    {
        await Task.Delay(5); // Simulate async operation

        var booking = _dataStore.Bookings.FirstOrDefault(b => b.Id == bookingId);
        return booking != null && CanCancelBooking(booking);
    }

    public async Task<bool> CanResidentCancelBookingAsync(Guid bookingId, Guid residentId)
    {
        await Task.Delay(5); // Simulate async operation

        var booking = _dataStore.Bookings.FirstOrDefault(b => b.Id == bookingId && b.ResidentId == residentId);
        return booking != null && CanCancelBooking(booking);
    }

    private bool CanCancelBooking(Booking booking)
    {
        // Can cancel if booking is at least 24 hours in the future
        var hoursUntilBooking = (booking.Date - DateTime.UtcNow).TotalHours;
        return hoursUntilBooking >= 24;
    }
}
