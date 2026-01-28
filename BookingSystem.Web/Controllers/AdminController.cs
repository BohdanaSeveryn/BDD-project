using BookingSystem.Web.DTOs;
using BookingSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IResidentService _residentService;
    private readonly IBookingService _bookingService;

    public AdminController(
        IAdminService adminService,
        IResidentService residentService,
        IBookingService bookingService)
    {
        _adminService = adminService;
        _residentService = residentService;
        _bookingService = bookingService;
    }

    [HttpGet("bookings")]
    public async Task<ActionResult<List<AdminBookingDetails>>> GetAllBookings([FromQuery] DateTime date)
    {
        var bookings = await _adminService.GetAllBookingsAsync(date);
        return Ok(bookings);
    }

    [HttpGet("bookings/{id}")]
    public async Task<ActionResult<AdminBookingDetails>> GetBookingDetails(Guid id)
    {
        var booking = await _adminService.GetBookingDetailsAsync(id);
        if (booking == null)
            return NotFound();

        return Ok(booking);
    }

    [HttpDelete("bookings/{id}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid id, [FromBody] AdminCancellationRequest request)
    {
        var success = await _bookingService.CancelBookingAsAdminAsync(id, request.Reason);
        if (!success)
            return BadRequest(new { errorMessage = "Varauksen peruutus epäonnistui" });

        return Ok(new { message = "Varaus peruutettu" });
    }

    [HttpGet("residents")]
    public async Task<ActionResult<List<ResidentAccount>>> GetAllResidents()
    {
        var residents = await _residentService.GetAllResidentsAsync();
        return Ok(residents);
    }

    [HttpPost("residents")]
    public async Task<ActionResult<ResidentAccount>> CreateResident([FromBody] CreateResidentRequest request)
    {
        var resident = await _residentService.CreateResidentAsync(request);
        if (resident == null)
            return BadRequest(new { errorMessage = "Asukaan luominen epäonnistui" });

        return Ok(resident);
    }

    [HttpPut("residents/{id}")]
    public async Task<IActionResult> UpdateResident(Guid id, [FromBody] UpdateResidentRequest request)
    {
        var success = await _adminService.UpdateResidentAsync(id, request);
        if (!success)
            return NotFound();

        return Ok(new { message = "Asukas päivitetty onnistuneesti" });
    }

    [HttpDelete("residents/{id}")]
    public async Task<IActionResult> DeleteResident(Guid id)
    {
        var success = await _adminService.DeleteResidentAsync(id);
        if (!success)
            return NotFound();

        return Ok(new { message = "Asukas poistettu onnistuneesti" });
    }
}
