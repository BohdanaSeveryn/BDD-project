using BookingSystem.Web.DTOs;
using BookingSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("availability")]
    public async Task<ActionResult<List<TimeSlotResponse>>> GetAvailability([FromQuery] Guid facilityId, [FromQuery] DateTime date)
    {
        var slots = await _bookingService.GetAvailabilityAsync(facilityId, date);
        return Ok(slots);
    }

    [HttpPost("confirm")]
    [Authorize]
    public async Task<ActionResult<BookingResponse>> ConfirmBooking([FromBody] CreateBookingRequest request)
    {
        var booking = await _bookingService.ConfirmBookingAsync(request);
        if (booking == null)
            return BadRequest(new { errorMessage = "Varauksen vahvistaminen epäonnistui" });

        return Ok(booking);
    }

    [HttpGet("my-bookings/{residentId}")]
    [Authorize]
    public async Task<ActionResult<List<BookingResponse>>> GetMyBookings(Guid residentId)
    {
        var bookings = await _bookingService.GetUpcomingBookingsAsync(residentId);
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<BookingDetails>> GetBookingDetails(Guid id)
    {
        var booking = await _bookingService.GetBookingDetailsAsync(id);
        if (booking == null)
            return NotFound();

        return Ok(booking);
    }

    [HttpDelete("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelBooking(Guid id, [FromBody] dynamic request)
    {
        var residentId = Guid.Parse(request.residentId);
        var success = await _bookingService.CancelBookingAsync(id, residentId);
        if (!success)
            return BadRequest(new { errorMessage = "Varauksen peruutus epäonnistui" });

        return Ok(new { message = "Varaus peruutettu" });
    }
}
