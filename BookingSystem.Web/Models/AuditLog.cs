namespace BookingSystem.Web.Models;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
