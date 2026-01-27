using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;
using BookingSystem.Services;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IEmailService _emailService;
    private readonly ITwoFactorService _twoFactorService;

    public AdminController(
        IAdminService adminService,
        IEmailService emailService,
        ITwoFactorService twoFactorService)
    {
        _adminService = adminService;
        _emailService = emailService;
        _twoFactorService = twoFactorService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            return BadRequest("Username and password are required");

        var isAuthenticated = await _adminService.AuthenticateAsync(request.Username, request.Password);
        
        if (!isAuthenticated)
            return Unauthorized("Invalid credentials");

        // Send 2FA code
        await _twoFactorService.SendTwoFactorCodeAsync("admin@example.com");

        return Ok(new { message = "2FA code sent to email", requiresTwoFactor = true });
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> Verify2FA([FromBody] Verify2FARequest request)
    {
        var isValid = await _twoFactorService.ValidateTwoFactorCodeAsync(request.AdminId, request.Code);
        
        if (!isValid)
            return Unauthorized("Invalid 2FA code");

        return Ok(new { message = "2FA verified", token = Guid.NewGuid().ToString() });
    }

    [HttpPost("residents")]
    public async Task<IActionResult> CreateResident([FromBody] CreateResidentRequest request, [FromQuery] Guid adminId)
    {
        var isAuthorized = await _adminService.IsAdminAuthorizedAsync(adminId);
        
        if (!isAuthorized)
            return Forbid();

        var resident = await _adminService.CreateResidentAsync(request);
        
        // Send activation email
        await _emailService.SendActivationEmailAsync(
            resident.Email,
            $"https://localhost:5001/api/residents/activate?token={resident.ActivationToken}");

        return CreatedAtAction("GetResident", "Residents", new { id = resident.Id }, resident);
    }

    [HttpDelete("residents/{residentId}")]
    public async Task<IActionResult> DeleteResident(Guid residentId, [FromQuery] Guid adminId)
    {
        var isAuthorized = await _adminService.IsAdminAuthorizedAsync(adminId);
        
        if (!isAuthorized)
            return Forbid();

        var resident = await _adminService.GetResidentByIdAsync(residentId);
        
        if (resident == null)
            return NotFound();

        var result = await _adminService.DeleteResidentAccountAsync(residentId);
        
        if (!result)
            return BadRequest("Failed to delete resident");

        // Send deletion notification
        await _emailService.SendAccountDeletionEmailAsync(resident.Email);

        return Ok(new { message = "Resident deleted successfully" });
    }

    [HttpPut("residents/{residentId}")]
    public async Task<IActionResult> UpdateResident(
        Guid residentId,
        [FromBody] UpdateResidentRequest request,
        [FromQuery] Guid adminId)
    {
        var isAuthorized = await _adminService.IsAdminAuthorizedAsync(adminId);
        
        if (!isAuthorized)
            return Forbid();

        var result = await _adminService.UpdateResidentAsync(residentId, request);
        
        if (!result)
            return NotFound();

        return Ok(new { message = "Resident updated successfully" });
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetAllBookings([FromQuery] DateTime date, [FromQuery] Guid adminId)
    {
        var isAuthorized = await _adminService.IsAdminAuthorizedAsync(adminId);
        
        if (!isAuthorized)
            return Forbid();

        var bookings = await _adminService.GetAllBookingsAsync(date);
        return Ok(bookings);
    }

    [HttpGet("bookings/{id}")]
    public async Task<IActionResult> GetBookingDetails(Guid id, [FromQuery] Guid adminId)
    {
        var isAuthorized = await _adminService.IsAdminAuthorizedAsync(adminId);
        
        if (!isAuthorized)
            return Forbid();

        var details = await _adminService.GetBookingDetailsAsync(id);
        return Ok(details);
    }

    [HttpDelete("bookings/{bookingId}")]
    public async Task<IActionResult> CancelBooking(
        Guid bookingId,
        [FromBody] AdminCancelBookingRequest request,
        [FromQuery] Guid adminId)
    {
        var isAuthorized = await _adminService.IsAdminAuthorizedAsync(adminId);
        
        if (!isAuthorized)
            return Forbid();

        var adminRequest = new AdminCancellationRequest
        {
            BookingId = bookingId,
            Reason = request.Reason,
            AdminId = adminId
        };

        var result = await _adminService.CancelBookingAsync(adminRequest);
        
        if (!result)
            return NotFound();

        return Ok(new { message = "Booking cancelled successfully" });
    }

    [HttpGet("residents/{residentId}")]
    public async Task<IActionResult> GetResident(Guid residentId, [FromQuery] Guid adminId)
    {
        var isAuthorized = await _adminService.IsAdminAuthorizedAsync(adminId);
        
        if (!isAuthorized)
            return Forbid();

        var resident = await _adminService.GetResidentByIdAsync(residentId);
        
        if (resident == null)
            return NotFound();

        return Ok(resident);
    }
}

public class AdminLoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class Verify2FARequest
{
    public Guid AdminId { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class AdminCancelBookingRequest
{
    public string Reason { get; set; } = string.Empty;
}
