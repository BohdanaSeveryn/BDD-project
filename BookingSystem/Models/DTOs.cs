using System;
using System.Collections.Generic;

namespace BookingSystem.Models;

/// <summary>
/// Data Transfer Objects and Request/Response models
/// </summary>

public class CreateResidentRequest
{
    public string Name { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class UpdateResidentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
}

public class LoginResult
{
    public bool Success { get; set; }
    public Guid ResidentId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

public class CreateBookingRequest
{
    public Guid ResidentId { get; set; }
    public Guid TimeSlotId { get; set; }
}

public class AdminCancellationRequest
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid AdminId { get; set; }
}

public class BookingSession
{
    public Guid? SelectedSlotId { get; set; }
}

public class BookingDetails
{
    public Guid Id { get; set; }
    public string Facility { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public bool CanManage { get; set; }
    public bool CanCancelBooking { get; set; }
}

public class AdminBookingDetails
{
    public Guid Id { get; set; }
    public string ResidentName { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public bool DataIncomplete { get; set; }
    public string? ErrorMessage { get; set; }
}

public class BookingCalendarItem
{
    public Guid Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsBooked { get; set; }
    public string? ResidentName { get; set; }
    public string? DataError { get; set; }
}

public class BookingConfirmationEmail
{
    public string ResidentEmail { get; set; } = string.Empty;
    public string FacilityName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public string ConfirmationToken { get; set; } = string.Empty;
    public string CancellationLink { get; set; } = string.Empty;
}

public class AccountDeletionEmail
{
    public string ResidentEmail { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime DeletionDate { get; set; }
}

public class ActivationEmail
{
    public string ResidentEmail { get; set; } = string.Empty;
    public string ActivationLink { get; set; } = string.Empty;
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
