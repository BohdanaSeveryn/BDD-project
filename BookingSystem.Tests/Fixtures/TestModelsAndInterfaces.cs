namespace BookingSystem.Tests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Service interfaces for mocking
/// </summary>

public interface IResidentService
{
    Task<bool> ActivateAccountAsync(Guid residentId, string password);
    Task<bool> IsActivationTokenValidAsync(string token);
    Task<LoginResult> LoginAsync(string apartmentNumber, string password);
    Task<ResidentAccount> GetResidentByIdAsync(Guid residentId);
    Task<ResidentAccount> CreateResidentAsync(CreateResidentRequest request);
}

public interface IAdminService
{
    Task<bool> IsAdminAuthorizedAsync(Guid adminId);
    Task<bool> AuthenticateAsync(string username, string password);
    Task<ResidentAccount> CreateResidentAsync(CreateResidentRequest request);
    Task<bool> DeleteResidentAccountAsync(Guid residentId);
    Task<bool> UpdateResidentAsync(Guid residentId, UpdateResidentRequest request);
    Task<List<BookingCalendarItem>> GetAllBookingsAsync(DateTime bookingDate);
    Task<List<BookingCalendarItem>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<AdminBookingDetails> GetBookingDetailsAsync(Guid bookingId);
    Task<bool> CancelBookingAsync(AdminCancellationRequest request);
    Task<GenerateTimeSlotsResult> GenerateDailyTimeSlotsAsync(GenerateTimeSlotsRequest request);
    Task<List<FacilityItem>> GetFacilitiesAsync();
}

public interface IBookingService
{
    Task<List<TimeSlot>> GetAvailabilityAsync(Guid facilityId, DateTime bookingDate);
    Task<bool> SelectTimeSlotAsync(Guid timeSlotId);
    Task<bool> ClearSelectionAsync();
    Task<Booking> ConfirmBookingAsync(CreateBookingRequest request);
    Task<List<TimeSlot>> RefreshAvailabilityAsync(Guid facilityId, DateTime bookingDate);
    Task<List<Booking>> GetMyBookingsAsync(Guid residentId, DateTime bookingDate);
    Task<List<Booking>> GetCalendarBookingsAsync(Guid facilityId, DateTime bookingDate);
    Task<BookingDetails> GetBookingDetailsAsync(Guid bookingId);
    Task<bool> IsUserAuthorizedAsync(Guid residentId);
    Task<List<Booking>> GetUpcomingBookingsAsync(Guid residentId);
    Task<bool> CancelBookingAsync(Guid bookingId, Guid residentId);
    Task<bool> CanCancelBookingAsync(Guid bookingId);
    Task<bool> CanResidentCancelBookingAsync(Guid bookingId, Guid residentId);
    Task<List<FacilityItem>> GetFacilitiesAsync();
}

public interface IEmailService
{
    Task<bool> SendActivationEmailAsync(string email, string activationLink);
    Task<bool> SendAccountDeletionEmailAsync(string email);
    Task<bool> SendBookingConfirmationAsync(BookingConfirmationEmail confirmation);
    Task<bool> SendCancellationEmailAsync(string email, string reason);
    Task<bool> SendAccountDeletionNotificationAsync(AccountDeletionEmail notification);
    Task<bool> SendEmailChangeNotificationAsync(string newEmail);
    Task<bool> SendCancellationNotificationAsync(string email, string reason);
    Task<bool> SendActivationEmailAsync(string email, string activationLink, string cancelLink);
}

public interface ITwoFactorService
{
    Task<bool> SendTwoFactorCodeAsync(string email);
    Task<bool> ValidateTwoFactorCodeAsync(Guid adminId, string code);
}

public interface IAuthenticationService
{
    Task<bool> IsUserAuthenticatedAsync(Guid userId);
}

public interface INotificationService
{
    Task<bool> NotifyResidentOfCancellationAsync(string email, string reason);
    Task<bool> SendCancellationEmailAsync(string email, string reason);
}

/// <summary>
/// /// Service interfaces for mocking
/// </summary>

public class ResidentAccount
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ApartmentNumber { get; set; }
    public bool IsActive { get; set; }
    public string ActivationToken { get; set; }
    public DateTime? ActivationTokenExpiry { get; set; }
}

public class LoginResult
{
    public bool Success { get; set; }
    public Guid ResidentId { get; set; }
    public string Token { get; set; }
    public string ErrorMessage { get; set; }
}

public class CreateResidentRequest
{
    public string Name { get; set; }
    public string ApartmentNumber { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class UpdateResidentRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ApartmentNumber { get; set; }
}

public class TimeSlot
{
    public Guid Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public string Status { get; set; }
    public string Color { get; set; }
}

public class Booking
{
    public Guid Id { get; set; }
    public Guid ResidentId { get; set; }
    public Guid TimeSlotId { get; set; }
    public string Facility { get; set; }
    public DateTime Date { get; set; }
    public string Time { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string DayOfWeek { get; set; }
    public bool IsMyBooking { get; set; }
    public string Icon { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BookingSession
{
    public Guid? SelectedSlotId { get; set; }
}

public class BookingDetails
{
    public Guid Id { get; set; }
    public string Facility { get; set; }
    public DateTime Date { get; set; }
    public string Time { get; set; }
    public bool CanManage { get; set; }
    public bool CanCancelBooking { get; set; }
}

public class AdminBookingDetails
{
    public Guid Id { get; set; }
    public string ResidentName { get; set; }
    public string ApartmentNumber { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime Date { get; set; }
    public string Time { get; set; }
    public bool DataIncomplete { get; set; }
    public string ErrorMessage { get; set; }
}

public class BookingCalendarItem
{
    public Guid Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsBooked { get; set; }
    public string ResidentName { get; set; }
    public string DataError { get; set; }
}

public class FacilityItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
}

public class BookingConfirmationEmail
{
    public string ResidentEmail { get; set; }
    public string FacilityName { get; set; }
    public DateTime Date { get; set; }
    public string Time { get; set; }
    public string ConfirmationToken { get; set; }
    public string CancellationLink { get; set; }
}

public class AccountDeletionEmail
{
    public string ResidentEmail { get; set; }
    public string Message { get; set; }
    public DateTime DeletionDate { get; set; }
}

public class ActivationEmail
{
    public string ResidentEmail { get; set; }
    public string ActivationLink { get; set; }
}

public class AdminCancellationRequest
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; }
    public Guid AdminId { get; set; }
}

public class GenerateTimeSlotsRequest
{
    public Guid AdminId { get; set; }
    public Guid FacilityId { get; set; }
    public DateTime Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}

public class GenerateTimeSlotsResult
{
    public Guid FacilityId { get; set; }
    public DateTime Date { get; set; }
    public int CreatedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<TimeSlot> Slots { get; set; } = new();
}

public class CreateBookingRequest
{
    public Guid ResidentId { get; set; }
    public Guid TimeSlotId { get; set; }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
