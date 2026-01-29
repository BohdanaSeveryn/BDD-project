using BookingSystem.Web.DTOs;
using BookingSystem.Web.Services;
using BookingSystem.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly AppDbContext _context;

    public BookingsController(IBookingService bookingService, AppDbContext context)
    {
        _bookingService = bookingService;
        _context = context;
    }

    [HttpGet("facilities")]
    public async Task<ActionResult<List<FacilityResponse>>> GetFacilities()
    {
        var facilities = await _context.Facilities
            .Where(f => f.IsAvailable)
            .OrderBy(f => f.Name)
            .Select(f => new FacilityResponse
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Icon = f.Icon
            })
            .ToListAsync();

        return Ok(facilities);
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
