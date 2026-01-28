using BookingSystem.Web.DTOs;

namespace BookingSystem.Web.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;

    public NotificationService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task<bool> NotifyBookingConfirmedAsync(string residentEmail, BookingConfirmationEmail confirmation)
    {
        return await _emailService.SendBookingConfirmationAsync(confirmation);
    }

    public async Task<bool> NotifyBookingCancelledAsync(string residentEmail, string residentName, string reason)
    {
        return await _emailService.SendCancellationNotificationAsync(residentEmail, reason);
    }
}
