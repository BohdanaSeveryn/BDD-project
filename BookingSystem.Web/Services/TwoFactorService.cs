using BookingSystem.Web.Data;
using BookingSystem.Web.Models;
using BookingSystem.Web.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Web.Services;

public class TwoFactorService : ITwoFactorService
{
    private readonly AppDbContext _context;
    private readonly EmailValidator _emailValidator;
    private readonly IEmailService _emailService;
    private static readonly Dictionary<Guid, (string Code, DateTime Expiry)> _2FACache = new();

    public TwoFactorService(AppDbContext context, EmailValidator emailValidator, IEmailService emailService)
    {
        _context = context;
        _emailValidator = emailValidator;
        _emailService = emailService;
    }

    public async Task<string> GenerateAndSendCodeAsync(Guid adminId, string email)
    {
        var code = _emailValidator.Generate2FACode();
        var expiry = DateTime.UtcNow.AddMinutes(5);

        _2FACache[adminId] = (code, expiry);

        // In a real application, send via email
        // For testing, the code is stored in cache
        await Task.CompletedTask;
        return code;
    }

    public async Task<bool> VerifyCodeAsync(Guid adminId, string code)
    {
        await Task.CompletedTask;

        if (!_2FACache.TryGetValue(adminId, out var codeData))
            return false;

        if (DateTime.UtcNow > codeData.Expiry)
        {
            _2FACache.Remove(adminId);
            return false;
        }

        if (codeData.Code != code)
            return false;

        _2FACache.Remove(adminId);
        return true;
    }

    public async Task<bool> SetupTwoFactorAsync(Guid adminId)
    {
        var admin = await _context.Admins.FindAsync(adminId);
        if (admin == null)
            return false;

        admin.TwoFactorEnabled = true;
        admin.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
