using System;

namespace BookingSystem.Models;

/// <summary>
/// Resident account model
/// </summary>
public class ResidentAccount
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string ActivationToken { get; set; } = string.Empty;
    public DateTime? ActivationTokenExpiry { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AdminAccount
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TimeSlot
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public string? Status { get; set; }
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Booking
{
    public Guid Id { get; set; }
    public Guid ResidentId { get; set; }
    public Guid TimeSlotId { get; set; }
    public string Facility { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public bool IsMyBooking { get; set; }
    public string? Icon { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}

public class Facility
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class TwoFactorCode
{
    public Guid Id { get; set; }
    public Guid AdminId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiryTime { get; set; }
    public bool IsUsed { get; set; }
}
