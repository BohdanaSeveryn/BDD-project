using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingSystem.Models;

namespace BookingSystem.Services;

/// <summary>
/// Service for sending emails
/// Handles activation emails, booking confirmations, cancellations, and notifications
/// </summary>
public class EmailService : IEmailService
{
    private readonly List<string> _sentEmails = new();

    public async Task<bool> SendActivationEmailAsync(string email, string activationLink)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(activationLink))
            return false;

        _sentEmails.Add($"Activation email to {email}");
        return true;
    }

    public async Task<bool> SendAccountDeletionEmailAsync(string email)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(email))
            return false;

        _sentEmails.Add($"Account deletion notification to {email}");
        return true;
    }

    public async Task<bool> SendBookingConfirmationAsync(BookingConfirmationEmail confirmation)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(confirmation.ResidentEmail) || string.IsNullOrEmpty(confirmation.FacilityName))
            return false;

        _sentEmails.Add($"Booking confirmation to {confirmation.ResidentEmail} for {confirmation.FacilityName}");
        return true;
    }

    public async Task<bool> SendCancellationEmailAsync(string email, string reason)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(email))
            return false;

        _sentEmails.Add($"Cancellation email to {email}. Reason: {reason}");
        return true;
    }

    public async Task<bool> SendAccountDeletionNotificationAsync(AccountDeletionEmail notification)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(notification.ResidentEmail))
            return false;

        _sentEmails.Add($"Account deletion notification to {notification.ResidentEmail}");
        return true;
    }

    public async Task<bool> SendEmailChangeNotificationAsync(string newEmail)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(newEmail))
            return false;

        _sentEmails.Add($"Email change notification to {newEmail}");
        return true;
    }

    public async Task<bool> SendCancellationNotificationAsync(string email, string reason)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(email))
            return false;

        _sentEmails.Add($"Cancellation notification to {email}. Reason: {reason}");
        return true;
    }

    public async Task<bool> SendActivationEmailAsync(string email, string activationLink, string cancelLink)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(activationLink))
            return false;

        _sentEmails.Add($"Activation email to {email} with cancel link");
        return true;
    }

    public IReadOnlyList<string> GetSentEmails() => _sentEmails.AsReadOnly();

    public void ClearSentEmails() => _sentEmails.Clear();
}
