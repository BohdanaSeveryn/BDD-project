using BookingSystem.Web.Data;
using BookingSystem.Web.DTOs;
using BookingSystem.Web.Models;
using BookingSystem.Web.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Web.Services;

public class ResidentService : IResidentService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher;
    private readonly EmailValidator _emailValidator;
    private readonly IEmailService _emailService;

    public ResidentService(
        AppDbContext context,
        PasswordHasher passwordHasher,
        EmailValidator emailValidator,
        IEmailService emailService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _emailValidator = emailValidator;
        _emailService = emailService;
    }

    public async Task<ResidentAccount?> CreateResidentAsync(CreateResidentRequest request)
    {
        if (!_emailValidator.IsValidEmail(request.Email))
            return null;

        var existingResident = await _context.Residents
            .FirstOrDefaultAsync(r => r.Email == request.Email || r.ApartmentNumber == request.ApartmentNumber);

        if (existingResident != null)
            return null;

        var resident = new Resident
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            ApartmentNumber = request.ApartmentNumber,
            IsActive = false,
            ActivationToken = _emailValidator.GenerateActivationToken(),
            ActivationTokenExpiry = DateTime.UtcNow.AddHours(24)
        };

        await _context.Residents.AddAsync(resident);
        await _context.SaveChangesAsync();

        await SendActivationEmailAsync(resident.Id, resident.Email, resident.ActivationToken);

        return new ResidentAccount
        {
            Id = resident.Id,
            Name = resident.Name,
            Email = resident.Email,
            ApartmentNumber = resident.ApartmentNumber,
            IsActive = resident.IsActive
        };
    }

    public async Task<bool> DeleteResidentAsync(Guid residentId)
    {
        var resident = await _context.Residents.FindAsync(residentId);
        if (resident == null)
            return false;

        resident.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _emailService.SendAccountDeletionEmailAsync(resident.Email, resident.Name);

        return true;
    }

    public async Task<bool> UpdateResidentAsync(Guid residentId, UpdateResidentRequest request)
    {
        var resident = await _context.Residents.FindAsync(residentId);
        if (resident == null)
            return false;

        resident.Name = request.Name;
        resident.Email = request.Email;
        resident.Phone = request.Phone;
        resident.ApartmentNumber = request.ApartmentNumber;
        resident.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ResidentAccount?> GetResidentByIdAsync(Guid residentId)
    {
        var resident = await _context.Residents.FindAsync(residentId);
        if (resident == null)
            return null;

        return new ResidentAccount
        {
            Id = resident.Id,
            Name = resident.Name,
            Email = resident.Email,
            ApartmentNumber = resident.ApartmentNumber,
            IsActive = resident.IsActive
        };
    }

    public async Task<List<ResidentAccount>> GetAllResidentsAsync()
    {
        var residents = await _context.Residents
            .Where(r => r.DeletedAt == null)
            .ToListAsync();

        return residents.Select(r => new ResidentAccount
        {
            Id = r.Id,
            Name = r.Name,
            Email = r.Email,
            ApartmentNumber = r.ApartmentNumber,
            IsActive = r.IsActive
        }).ToList();
    }

    public async Task<bool> IsActivationTokenValidAsync(string token)
    {
        var resident = await _context.Residents
            .FirstOrDefaultAsync(r => r.ActivationToken == token);

        return resident != null && resident.ActivationTokenExpiry > DateTime.UtcNow;
    }

    public async Task SendActivationEmailAsync(Guid residentId, string email, string activationToken)
    {
        var activationLink = $"https://localhost:7001/activation.html?token={activationToken}";
        await _emailService.SendActivationEmailAsync(email, activationLink);
    }
}
