using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;
using BookingSystem.Services;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResidentsController : ControllerBase
{
    private readonly IResidentService _residentService;
    private readonly IEmailService _emailService;

    public ResidentsController(IResidentService residentService, IEmailService emailService)
    {
        _residentService = residentService;
        _emailService = emailService;
    }

    [HttpPost("activate")]
    public async Task<IActionResult> ActivateAccount([FromBody] ActivateAccountRequest request)
    {
        if (string.IsNullOrEmpty(request.Password))
            return BadRequest("Password is required");

        var result = await _residentService.ActivateAccountAsync(request.ResidentId, request.Password);
        
        if (!result)
            return NotFound("Resident not found or activation failed");

        return Ok(new { message = "Account activated successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.ApartmentNumber) || string.IsNullOrEmpty(request.Password))
            return BadRequest("Apartment number and password are required");

        var result = await _residentService.LoginAsync(request.ApartmentNumber, request.Password);
        
        if (!result.Success)
            return Unauthorized(result.ErrorMessage);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResident(Guid id)
    {
        var resident = await _residentService.GetResidentByIdAsync(id);
        
        if (resident == null)
            return NotFound();

        return Ok(resident);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterResident([FromBody] CreateResidentRequest request)
    {
        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) || 
            string.IsNullOrEmpty(request.ApartmentNumber))
            return BadRequest("Name, email, and apartment number are required");

        var resident = await _residentService.CreateResidentAsync(request);
        
        // Send activation email
        await _emailService.SendActivationEmailAsync(
            resident.Email,
            $"https://localhost:5001/api/residents/activate?token={resident.ActivationToken}");

        return CreatedAtAction(nameof(GetResident), new { id = resident.Id }, resident);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResident(Guid id, [FromBody] UpdateResidentRequest request)
    {
        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email))
            return BadRequest("Name and email are required");

        var result = await _residentService.UpdateResidentAsync(id, request);
        
        if (!result)
            return NotFound();

        return Ok(new { message = "Resident updated successfully" });
    }
}

public class ActivateAccountRequest
{
    public Guid ResidentId { get; set; }
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string ApartmentNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
