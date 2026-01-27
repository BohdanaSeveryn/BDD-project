using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingSystem.Models;

namespace BookingSystem.Services;

/// <summary>
/// Service interfaces
/// </summary>

public interface IResidentService
{
    Task<bool> ActivateAccountAsync(Guid residentId, string password);
    Task<bool> IsActivationTokenValidAsync(string token);
    Task<LoginResult> LoginAsync(string apartmentNumber, string password);
    Task<ResidentAccount> GetResidentByIdAsync(Guid residentId);
    Task<ResidentAccount> CreateResidentAsync(CreateResidentRequest request);
    Task<bool> UpdateResidentAsync(Guid residentId, UpdateResidentRequest request);
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
    Task<ResidentAccount?> GetResidentByIdAsync(Guid residentId);
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
