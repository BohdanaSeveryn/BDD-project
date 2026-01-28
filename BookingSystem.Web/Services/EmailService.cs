using BookingSystem.Web.Data;
using BookingSystem.Web.DTOs;
using BookingSystem.Web.Models;
using BookingSystem.Web.Utilities;

namespace BookingSystem.Web.Services;

public class EmailService : IEmailService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public EmailService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<bool> SendActivationEmailAsync(string email, string activationLink)
    {
        try
        {
            // In a real implementation, you would send actual emails via SMTP
            var emailLog = new EmailAuditLog
            {
                Id = Guid.NewGuid(),
                RecipientEmail = email,
                Subject = "Tilin aktivointi - Varauspalvelu",
                EmailType = "AccountActivation",
                Status = "Sent"
            };

            await _context.EmailAuditLogs.AddAsync(emailLog);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendBookingConfirmationAsync(BookingConfirmationEmail confirmation)
    {
        try
        {
            var emailLog = new EmailAuditLog
            {
                Id = Guid.NewGuid(),
                RecipientEmail = confirmation.ResidentEmail,
                Subject = "Varauksen vahvistus",
                EmailType = "BookingConfirmation",
                Status = "Sent"
            };

            await _context.EmailAuditLogs.AddAsync(emailLog);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendAccountDeletionEmailAsync(string email, string residentName)
    {
        try
        {
            var emailLog = new EmailAuditLog
            {
                Id = Guid.NewGuid(),
                RecipientEmail = email,
                Subject = "Tilin poisto - Varauspalvelu",
                EmailType = "AccountDeletion",
                Status = "Sent"
            };

            await _context.EmailAuditLogs.AddAsync(emailLog);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendCancellationNotificationAsync(string email, string reason)
    {
        try
        {
            var emailLog = new EmailAuditLog
            {
                Id = Guid.NewGuid(),
                RecipientEmail = email,
                Subject = "Varauksen peruutus",
                EmailType = "BookingCancellation",
                Status = "Sent"
            };

            await _context.EmailAuditLogs.AddAsync(emailLog);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}
