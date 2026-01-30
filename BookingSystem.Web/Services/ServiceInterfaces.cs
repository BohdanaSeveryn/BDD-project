using BookingSystem.Web.DTOs;

namespace BookingSystem.Web.Services;

public interface IAuthenticationService
{
    Task<LoginResponse?> LoginResidentAsync(string apartmentNumber, string password);
    Task<AdminLoginResponse?> LoginAdminAsync(string username, string password);
    Task<TwoFactorResponse?> VerifyTwoFactorCodeAsync(Guid adminId, string code);
    Task<bool> ActivateAccountAsync(string activationToken, string password);
}

public interface IResidentService
{
    Task<ResidentAccount?> CreateResidentAsync(CreateResidentRequest request);
    Task<bool> DeleteResidentAsync(Guid residentId);
    Task<bool> UpdateResidentAsync(Guid residentId, UpdateResidentRequest request);
    Task<ResidentAccount?> GetResidentByIdAsync(Guid residentId);
    Task<List<ResidentAccount>> GetAllResidentsAsync();
    Task<bool> IsActivationTokenValidAsync(string token);
    Task SendActivationEmailAsync(Guid residentId, string email, string activationToken);
}

public interface IBookingService
{
    Task<List<TimeSlotResponse>> GetAvailabilityAsync(Guid facilityId, DateTime bookingDate);
    Task<BookingResponse?> ConfirmBookingAsync(CreateBookingRequest request);
    Task<List<BookingResponse>> GetUpcomingBookingsAsync(Guid residentId);
    Task<BookingDetails?> GetBookingDetailsAsync(Guid bookingId);
    Task<bool> CancelBookingAsync(Guid bookingId, Guid residentId);
    Task<bool> CancelBookingAsAdminAsync(Guid bookingId, string reason);
}

public interface IAdminService
{
    Task<bool> AuthenticateAsync(string username, string password);
    Task<List<AdminBookingDetails>> GetAllBookingsAsync(DateTime bookingDate);
    Task<AdminBookingDetails?> GetBookingDetailsAsync(Guid bookingId);
    Task<bool> DeleteResidentAsync(Guid residentId);
    Task<bool> UpdateResidentAsync(Guid residentId, UpdateResidentRequest request);
    Task<GenerateTimeSlotsResponse?> GenerateDailyTimeSlotsAsync(Guid facilityId, DateTime date, TimeOnly startTime, TimeOnly endTime);
    Task<List<FacilityResponse>> GetFacilitiesAsync();
}

public interface IEmailService
{
    Task<bool> SendActivationEmailAsync(string email, string activationLink);
    Task<bool> SendBookingConfirmationAsync(BookingConfirmationEmail confirmation);
    Task<bool> SendAccountDeletionEmailAsync(string email, string residentName);
    Task<bool> SendCancellationNotificationAsync(string email, string reason);
}

public interface ITwoFactorService
{
    Task<string> GenerateAndSendCodeAsync(Guid adminId, string email);
    Task<bool> VerifyCodeAsync(Guid adminId, string code);
    Task<bool> SetupTwoFactorAsync(Guid adminId);
}

public interface INotificationService
{
    Task<bool> NotifyBookingConfirmedAsync(string residentEmail, BookingConfirmationEmail confirmation);
    Task<bool> NotifyBookingCancelledAsync(string residentEmail, string residentName, string reason);
}
