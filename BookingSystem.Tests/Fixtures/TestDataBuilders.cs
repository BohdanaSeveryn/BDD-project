using System;
using System.Collections.Generic;
using Xunit;

namespace BookingSystem.Tests.Fixtures;

/// <summary>
/// /// Helper classes for building test data
/// </summary>

public static class ResidentTestDataBuilder
{
    public static ResidentAccount BuildValidResident()
    {
        return new ResidentAccount
        {
            Id = Guid.NewGuid(),
            Name = "Test Resident",
            Email = "test@example.com",
            Phone = "+380501234567",
            ApartmentNumber = "A-101",
            IsActive = false,
            ActivationToken = Guid.NewGuid().ToString(),
            ActivationTokenExpiry = DateTime.UtcNow.AddHours(24)
        };
    }

    public static CreateResidentRequest BuildValidCreateRequest()
    {
        return new CreateResidentRequest
        {
            Name = "New Resident",
            ApartmentNumber = "B-202",
            Email = "new.resident@example.com",
            Phone = "+380509876543"
        };
    }

    public static UpdateResidentRequest BuildValidUpdateRequest()
    {
        return new UpdateResidentRequest
        {
            Name = "Updated name",
            Email = "updated@example.com",
            Phone = "+380505555555",
            ApartmentNumber = "C-303"
        };
    }
}

public static class BookingTestDataBuilder
{
    public static List<TimeSlot> BuildDailyTimeSlots()
    {
        return new List<TimeSlot>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0), IsAvailable = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsAvailable = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), IsAvailable = false },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(12, 0), IsAvailable = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(13, 0), IsAvailable = false },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(14, 0), IsAvailable = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(15, 0), IsAvailable = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(15, 0), EndTime = new TimeOnly(16, 0), IsAvailable = false },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(16, 0), EndTime = new TimeOnly(17, 0), IsAvailable = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(18, 0), IsAvailable = true }
        };
    }

    public static Booking BuildValidBooking(Guid residentId, Guid timeSlotId)
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            ResidentId = residentId,
            TimeSlotId = timeSlotId,
            Facility = "Laundry room",
            Date = DateTime.Today.AddDays(1),
            Time = "10:00 - 11:00",
            Status = "Confirmed",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static BookingConfirmationEmail BuildBookingConfirmationEmail(string email)
    {
        return new BookingConfirmationEmail
        {
            ResidentEmail = email,
            FacilityName = "Laundry room",
            Date = DateTime.Today.AddDays(1),
            Time = "10:00 - 11:00",
            ConfirmationToken = Guid.NewGuid().ToString(),
            CancellationLink = $"https://example.com/cancel-booking/{Guid.NewGuid()}"
        };
    }

    public static BookingDetails BuildBookingDetails()
    {
        return new BookingDetails
        {
            Id = Guid.NewGuid(),
            Facility = "Laundry room",
            Date = DateTime.Today.AddDays(1),
            Time = "10:00 - 11:00",
            CanManage = true,
            CanCancelBooking = true
        };
    }
}

public static class AdminTestDataBuilder
{
    public static AdminBookingDetails BuildAdminBookingDetails()
    {
        return new AdminBookingDetails
        {
            Id = Guid.NewGuid(),
            ResidentName = "Peter Sidorenko",
            ApartmentNumber = "A-101",
            Email = "petro@example.com",
            Phone = "+380501234567",
            Date = DateTime.Today.AddDays(1),
            Time = "10:00 - 11:00"
        };
    }

    public static AdminCancellationRequest BuildCancellationRequest(Guid adminId, Guid bookingId)
    {
        return new AdminCancellationRequest
        {
            BookingId = bookingId,
            Reason = "Maintenance",
            AdminId = adminId
        };
    }

    public static List<BookingCalendarItem> BuildMixedBookingItems()
    {
        return new List<BookingCalendarItem>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0), IsBooked = false },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsBooked = true, ResidentName = "Ivan" },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), IsBooked = false },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(12, 0), IsBooked = true, ResidentName = "Maria" }
        };
    }
}

/// <summary>
/// Collection Fixtures for shared test state
/// </summary>

[CollectionDefinition("Booking System Collection")]
public class BookingSystemCollection : ICollectionFixture<BookingSystemFixture>
{
    // This is only a collection definition for grouping tests
}

public class BookingSystemFixture : IDisposable
{
    public Guid TestResidentId { get; private set; }
    public Guid TestAdminId { get; private set; }
    public string TestFacilityId { get; private set; }
    public DateTime TestBookingDate { get; private set; }

    public BookingSystemFixture()
    {
        TestResidentId = Guid.NewGuid();
        TestAdminId = Guid.NewGuid();
        TestFacilityId = Guid.NewGuid().ToString();
        TestBookingDate = DateTime.Today.AddDays(1);
    }

    public void Dispose()
    {
        // Cleanup resources after tests
    }
}

/// <summary>
/// Test constants
/// </summary>

public static class TestConstants
{
    // Email addresses
    public const string ValidResidentEmail = "resident@example.com";
    public const string ValidAdminEmail = "admin@example.com";
    public const string InvalidEmail = "invalid-email";

    // Password
    public const string ValidPassword = "ValidPassword123!@#";
    public const string WeakPassword = "weak";
    public const string InvalidPassword = "wrong";

    // Apartment numbers
    public const string ValidApartmentNumber = "A-101";
    public const string AnotherApartmentNumber = "B-202";
    public const string InvalidApartmentNumber = "";

    // Phone numbers
    public const string ValidPhoneNumber = "+380501234567";
    public const string AnotherPhoneNumber = "+380509876543";
    public const string InvalidPhoneNumber = "123";

    // Tokens
    public const string ValidActivationToken = "valid-token-123456";
    public const string InvalidActivationToken = "invalid-token";
    public const string ValidTwoFactorCode = "123456";
    public const string InvalidTwoFactorCode = "000000";

    // URL
    public const string BaseUrl = "https://example.com";
    public const string ActivationLinkFormat = BaseUrl + "/activate?token={0}";
    public const string CancellationLinkFormat = BaseUrl + "/cancel-booking/{0}";

    // Cancellation reasons
    public const string MaintenanceReason = "Maintenance";
    public const string EmergencyReason = "Unforeseen circumstance";
    public const string ConflictReason = "Booking conflict";

    // Time slots
    public static readonly TimeOnly MorningSlotStart = new TimeOnly(8, 0);
    public static readonly TimeOnly AfternoonSlotStart = new TimeOnly(14, 0);
    public static readonly TimeOnly EveningSlotStart = new TimeOnly(18, 0);

    // Error messages
    public const string AccountNotActivatedError = "Account is not activated";
    public const string DuplicateEmailError = "Email already in use";
    public const string InvalidCredentialsError = "Invalid credentials";
    public const string SlotAlreadyBookedError = "This slot just reserved";
    public const string AccountNotFoundError = "Account not found";
}

/// <summary>
/// Comparers for complex types
/// </summary>

public class ResidentAccountComparer : IEqualityComparer<ResidentAccount>
{
    public bool Equals(ResidentAccount x, ResidentAccount y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;

        return x.Id == y.Id &&
               x.Email == y.Email &&
               x.ApartmentNumber == y.ApartmentNumber;
    }

    public int GetHashCode(ResidentAccount obj)
    {
        return HashCode.Combine(obj.Id, obj.Email, obj.ApartmentNumber);
    }
}

public class BookingComparer : IEqualityComparer<Booking>
{
    public bool Equals(Booking x, Booking y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;

        return x.Id == y.Id &&
               x.ResidentId == y.ResidentId &&
               x.TimeSlotId == y.TimeSlotId &&
               x.Date == y.Date;
    }

    public int GetHashCode(Booking obj)
    {
        return HashCode.Combine(obj.Id, obj.ResidentId, obj.TimeSlotId, obj.Date);
    }
}
