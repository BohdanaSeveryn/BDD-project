using BookingSystem.Web.Data;
using BookingSystem.Web.DTOs;
using BookingSystem.Web.Models;
using BookingSystem.Web.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Web.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly ITwoFactorService _twoFactorService;
    private readonly IEmailService _emailService;

    public AuthenticationService(
        AppDbContext context,
        PasswordHasher passwordHasher,
        JwtTokenGenerator jwtTokenGenerator,
        ITwoFactorService twoFactorService,
        IEmailService emailService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _twoFactorService = twoFactorService;
        _emailService = emailService;
    }

    public async Task<LoginResponse?> LoginResidentAsync(string apartmentNumber, string password)
    {
        var resident = await _context.Residents
            .FirstOrDefaultAsync(r => r.ApartmentNumber == apartmentNumber);

        if (resident == null || !resident.IsActive || string.IsNullOrEmpty(resident.PasswordHash))
            return null;

        if (!_passwordHasher.VerifyPassword(password, resident.PasswordHash))
            return null;

        var token = _jwtTokenGenerator.GenerateToken(resident.Id, "Resident", apartmentNumber: resident.ApartmentNumber);

        return new LoginResponse
        {
            Token = token,
            ResidentId = resident.Id,
            ApartmentNumber = resident.ApartmentNumber,
            Name = resident.Name
        };
    }

    public async Task<AdminLoginResponse?> LoginAdminAsync(string username, string password)
    {
        var admin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Username == username);

        if (admin == null || !admin.IsActive)
            return null;

        if (!_passwordHasher.VerifyPassword(password, admin.PasswordHash))
            return null;

        if (admin.TwoFactorEnabled)
        {
            await _twoFactorService.GenerateAndSendCodeAsync(admin.Id, admin.Email);
            return new AdminLoginResponse
            {
                RequiresTwoFactor = true,
                AdminId = admin.Id,
                Username = admin.Username
            };
        }

        var token = _jwtTokenGenerator.GenerateToken(admin.Id, "Admin", username: admin.Username);

        return new AdminLoginResponse
        {
            Token = token,
            AdminId = admin.Id,
            Username = admin.Username,
            RequiresTwoFactor = false
        };
    }

    public async Task<TwoFactorResponse?> VerifyTwoFactorCodeAsync(Guid adminId, string code)
    {
        var admin = await _context.Admins.FindAsync(adminId);
        if (admin == null)
            return null;

        if (!await _twoFactorService.VerifyCodeAsync(adminId, code))
            return null;

        var token = _jwtTokenGenerator.GenerateToken(admin.Id, "Admin", username: admin.Username);

        return new TwoFactorResponse
        {
            Token = token,
            AdminId = admin.Id
        };
    }

    public async Task<bool> ActivateAccountAsync(string activationToken, string password)
    {
        var resident = await _context.Residents
            .FirstOrDefaultAsync(r => r.ActivationToken == activationToken);

        if (resident == null || resident.ActivationTokenExpiry < DateTime.UtcNow)
            return false;

        resident.PasswordHash = _passwordHasher.HashPassword(password);
        resident.IsActive = true;
        resident.ActivationToken = null;
        resident.ActivationTokenExpiry = null;
        resident.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
