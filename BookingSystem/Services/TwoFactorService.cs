using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Models;
using BookingSystem.Repositories;

namespace BookingSystem.Services;

/// <summary>
/// Service for two-factor authentication
/// Handles sending and validating two-factor codes
/// </summary>
public class TwoFactorService : ITwoFactorService
{
    private readonly DataStore _dataStore;
    private readonly Random _random = new();

    public TwoFactorService(DataStore dataStore)
    {
        _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
    }

    public async Task<bool> SendTwoFactorCodeAsync(string email)
    {
        await Task.Delay(10); // Simulate async operation

        if (string.IsNullOrEmpty(email))
            return false;

        // Find admin with this email
        var admin = _dataStore.Admins.FirstOrDefault();
        if (admin == null)
            return false;

        var code = GenerateCode();
        var twoFactorCode = new TwoFactorCode
        {
            Id = Guid.NewGuid(),
            AdminId = admin.Id,
            Code = code,
            ExpiryTime = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        _dataStore.TwoFactorCodes.Add(twoFactorCode);
        return true;
    }

    public async Task<bool> ValidateTwoFactorCodeAsync(Guid adminId, string code)
    {
        await Task.Delay(10); // Simulate async operation

        var twoFactorCode = _dataStore.TwoFactorCodes.FirstOrDefault(
            x => x.AdminId == adminId && x.Code == code && !x.IsUsed);

        if (twoFactorCode == null)
            return false;

        if (twoFactorCode.ExpiryTime < DateTime.UtcNow)
            return false;

        twoFactorCode.IsUsed = true;
        return true;
    }

    private string GenerateCode()
    {
        return _random.Next(100000, 999999).ToString();
    }
}
