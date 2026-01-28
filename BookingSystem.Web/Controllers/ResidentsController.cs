using BookingSystem.Web.DTOs;
using BookingSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResidentsController : ControllerBase
{
    private readonly IResidentService _residentService;

    public ResidentsController(IResidentService residentService)
    {
        _residentService = residentService;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ResidentAccount>> GetResident(Guid id)
    {
        var resident = await _residentService.GetResidentByIdAsync(id);
        if (resident == null)
            return NotFound();

        return Ok(resident);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateResident(Guid id, [FromBody] UpdateResidentRequest request)
    {
        var success = await _residentService.UpdateResidentAsync(id, request);
        if (!success)
            return NotFound();

        return Ok(new { message = "Asukas päivitetty onnistuneesti" });
    }
}
