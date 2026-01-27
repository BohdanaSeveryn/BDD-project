using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Models;
using BookingSystem.Repositories;

namespace BookingSystem.Services;

/// <summary>
/// Service for resident account management
/// Handles account activation, login, and account creation
/// </summary>
public class ResidentService : IResidentService
{
    private readonly DataStore _dataStore;

    public ResidentService(DataStore dataStore)
    {
        _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
    }

    public async Task<bool> ActivateAccountAsync(Guid residentId, string password)
    {
        await Task.Delay(10); // Simulate async operation

        var resident = _dataStore.Residents.FirstOrDefault(r => r.Id == residentId);
        if (resident == null)
            return false;

        if (!await IsActivationTokenValidAsync(resident.ActivationToken))
            return false;

        resident.IsActive = true;
        resident.PasswordHash = DataStore.HashPassword(password);
        resident.ActivationToken = string.Empty;
        resident.ActivationTokenExpiry = null;
        resident.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public async Task<bool> IsActivationTokenValidAsync(string token)
    {
        await Task.Delay(5); // Simulate async operation

        var resident = _dataStore.Residents.FirstOrDefault(r => r.ActivationToken == token);
        if (resident == null)
            return false;

        return resident.ActivationTokenExpiry > DateTime.UtcNow;
    }

    public async Task<LoginResult> LoginAsync(string apartmentNumber, string password)
    {
        await Task.Delay(10); // Simulate async operation

        var resident = _dataStore.Residents.FirstOrDefault(r => r.ApartmentNumber == apartmentNumber);
        if (resident == null)
        {
            return new LoginResult 
            { 
                Success = false, 
                ErrorMessage = "Resident not found" 
            };
        }

        if (!resident.IsActive)
        {
            return new LoginResult 
            { 
                Success = false, 
                ErrorMessage = "Account not activated" 
            };
        }

        if (!DataStore.VerifyPassword(password, resident.PasswordHash ?? ""))
        {
            return new LoginResult 
            { 
                Success = false, 
                ErrorMessage = "Invalid password" 
            };
        }

        var token = GenerateAuthToken(resident.Id);
        return new LoginResult 
        { 
            Success = true, 
            ResidentId = resident.Id, 
            Token = token 
        };
    }

    public async Task<ResidentAccount> GetResidentByIdAsync(Guid residentId)
    {
        await Task.Delay(5); // Simulate async operation

        return _dataStore.Residents.FirstOrDefault(r => r.Id == residentId);
    }

    public async Task<ResidentAccount> CreateResidentAsync(CreateResidentRequest request)
    {
        await Task.Delay(10); // Simulate async operation

        var resident = new ResidentAccount
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ApartmentNumber = request.ApartmentNumber,
            Email = request.Email,
            Phone = request.Phone,
            IsActive = false,
            ActivationToken = GenerateActivationToken(),
            ActivationTokenExpiry = DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow
        };

        _dataStore.Residents.Add(resident);
        return resident;
    }

    public async Task<bool> UpdateResidentAsync(Guid residentId, UpdateResidentRequest request)
    {
        await Task.Delay(10); // Simulate async operation

        var resident = _dataStore.Residents.FirstOrDefault(r => r.Id == residentId);
        if (resident == null)
            return false;

        resident.Name = request.Name;
        resident.Email = request.Email;
        resident.Phone = request.Phone;
        resident.ApartmentNumber = request.ApartmentNumber;
        resident.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public async Task<bool> DeleteResidentAsync(Guid residentId)
    {
        await Task.Delay(10); // Simulate async operation

        var resident = _dataStore.Residents.FirstOrDefault(r => r.Id == residentId);
        if (resident == null)
            return false;

        _dataStore.Residents.Remove(resident);
        return true;
    }

    private static string GenerateActivationToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    private static string GenerateAuthToken(Guid residentId)
    {
        return $"token-{residentId:N}";
    }
}
