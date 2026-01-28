namespace BookingSystem.Web.Models;

public class Booking
{
    public Guid Id { get; set; }
    public Guid ResidentId { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid FacilityId { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Status { get; set; } = "Confirmed";
    public string? CancellationReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public virtual Resident Resident { get; set; } = null!;
    public virtual TimeSlot TimeSlot { get; set; } = null!;
    public virtual Facility Facility { get; set; } = null!;
}
