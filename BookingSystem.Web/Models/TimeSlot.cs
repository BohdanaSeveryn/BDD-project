namespace BookingSystem.Web.Models;

public class TimeSlot
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Status { get; set; } = "Available";
    public string Color { get; set; } = "green";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Facility Facility { get; set; } = null!;
    public virtual Booking? Booking { get; set; }
}
