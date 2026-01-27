using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;
using BookingSystem.Services;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;

    public BookingsController(
        IBookingService bookingService,
        IEmailService emailService,
        INotificationService notificationService)
    {
        _bookingService = bookingService;
        _emailService = emailService;
        _notificationService = notificationService;
    }

    [HttpGet("availability/{facilityId}")]
    public async Task<IActionResult> GetAvailability(Guid facilityId, [FromQuery] DateTime date)
    {
        var slots = await _bookingService.GetAvailabilityAsync(facilityId, date);
        return Ok(slots);
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            var booking = await _bookingService.ConfirmBookingAsync(request);
            
            if (booking == null)
                return BadRequest("Failed to confirm booking");

            // Send confirmation email
            var confirmation = new BookingConfirmationEmail
            {
                ResidentEmail = "resident@example.com",
                FacilityName = booking.Facility,
                Date = booking.Date,
                Time = booking.Time,
                ConfirmationToken = Guid.NewGuid().ToString()
            };
            
            await _emailService.SendBookingConfirmationAsync(confirmation);

            return CreatedAtAction(nameof(GetBookingDetails), new { id = booking.Id }, booking);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingDetails(Guid id)
    {
        var details = await _bookingService.GetBookingDetailsAsync(id);
        
        if (details == null)
            return NotFound();

        return Ok(details);
    }

    [HttpGet("resident/{residentId}")]
    public async Task<IActionResult> GetResidentBookings(Guid residentId, [FromQuery] DateTime? date = null)
    {
        var bookingDate = date ?? DateTime.UtcNow;
        var bookings = await _bookingService.GetMyBookingsAsync(residentId, bookingDate);
        return Ok(bookings);
    }

    [HttpGet("upcoming/{residentId}")]
    public async Task<IActionResult> GetUpcomingBookings(Guid residentId)
    {
        var bookings = await _bookingService.GetUpcomingBookingsAsync(residentId);
        return Ok(bookings);
    }

    [HttpDelete("{id}/resident/{residentId}")]
    public async Task<IActionResult> CancelBooking(Guid id, Guid residentId, [FromBody] CancellationRequest request)
    {
        var canCancel = await _bookingService.CanResidentCancelBookingAsync(id, residentId);
        
        if (!canCancel)
            return BadRequest("Booking cannot be cancelled. Cancellation window has expired.");

        var result = await _bookingService.CancelBookingAsync(id, residentId);
        
        if (!result)
            return NotFound();

        // Send cancellation notification
        await _notificationService.SendCancellationEmailAsync(
            "resident@example.com",
            request.Reason ?? "No reason provided");

        return Ok(new { message = "Booking cancelled successfully" });
    }

    [HttpPost("select-slot")]
    public async Task<IActionResult> SelectTimeSlot([FromBody] SelectSlotRequest request)
    {
        var result = await _bookingService.SelectTimeSlotAsync(request.TimeSlotId);
        
        if (!result)
            return BadRequest("Time slot is not available");

        return Ok(new { message = "Time slot selected" });
    }

    [HttpPost("clear-selection")]
    public async Task<IActionResult> ClearSelection()
    {
        var result = await _bookingService.ClearSelectionAsync();
        return Ok(new { message = "Selection cleared" });
    }
}

public class CancellationRequest
{
    public string? Reason { get; set; }
}

public class SelectSlotRequest
{
    public Guid TimeSlotId { get; set; }
}
