using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Models;
using BookingSystem.Repositories;

namespace BookingSystem.Services;

/// <summary>
/// Service for sending notifications to residents
/// Handles booking cancellation notifications
/// </summary>
public class NotificationService : INotificationService
{
    private readonly List<string> _sentNotifications = new();

    public async Task<bool> NotifyResidentOfCancellationAsync(string email, string reason)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(email))
            return false;

        _sentNotifications.Add($"Cancellation notification to {email}. Reason: {reason}");
        return true;
    }

    public async Task<bool> SendCancellationEmailAsync(string email, string reason)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(email))
            return false;

        _sentNotifications.Add($"Cancellation email to {email}. Reason: {reason}");
        return true;
    }

    public IReadOnlyList<string> GetSentNotifications() => _sentNotifications.AsReadOnly();

    public void ClearNotifications() => _sentNotifications.Clear();
}

/// <summary>
/// Service for authentication verification
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly DataStore _dataStore;

    public AuthenticationService(DataStore dataStore)
    {
        _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
    }

    public async Task<bool> IsUserAuthenticatedAsync(Guid userId)
    {
        await Task.Delay(5); // Simulate async operation

        var resident = _dataStore.Residents.FirstOrDefault(r => r.Id == userId && r.IsActive);
        return resident != null;
    }
}
