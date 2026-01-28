using BookingSystem.Web.DTOs;
using BookingSystem.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IResidentService _residentService;
    private readonly ITwoFactorService _twoFactorService;

    public AuthController(
        IAuthenticationService authService,
        IResidentService residentService,
        ITwoFactorService twoFactorService)
    {
        _authService = authService;
        _residentService = residentService;
        _twoFactorService = twoFactorService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginResidentAsync(request.ApartmentNumber, request.Password);
        if (result == null)
            return Unauthorized(new { errorMessage = "Virheellinen huoneistönumero tai salasana" });

        return Ok(result);
    }

    [HttpPost("admin-login")]
    public async Task<ActionResult<AdminLoginResponse>> AdminLogin([FromBody] AdminLoginRequest request)
    {
        var result = await _authService.LoginAdminAsync(request.Username, request.Password);
        if (result == null)
            return Unauthorized(new { errorMessage = "Virheellinen käyttäjänimi tai salasana" });

        return Ok(result);
    }

    [HttpPost("2fa/verify")]
    public async Task<ActionResult<TwoFactorResponse>> VerifyTwoFactor([FromBody] TwoFactorRequest request)
    {
        if (string.IsNullOrEmpty(request.Code))
            return BadRequest(new { errorMessage = "2FA-koodi vaaditaan" });

        // In a real app, you'd get the adminId from session/context
        var adminId = Guid.Parse(HttpContext.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());

        var result = await _authService.VerifyTwoFactorCodeAsync(adminId, request.Code);
        if (result == null)
            return Unauthorized(new { errorMessage = "Virheellinen 2FA-koodi" });

        return Ok(result);
    }

    [HttpPost("activate")]
    public async Task<IActionResult> ActivateAccount([FromQuery] string token, [FromBody] ActivationRequest request)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(request.Password))
            return BadRequest(new { errorMessage = "Token ja salasana vaaditaan" });

        var success = await _authService.ActivateAccountAsync(token, request.Password);
        if (!success)
            return BadRequest(new { errorMessage = "Aktivointi epäonnistui. Linkki on ehkä vanhentunut." });

        return Ok(new { message = "Tili aktivoitu onnistuneesti" });
    }

    [HttpPost("2fa/send")]
    public IActionResult Send2FACode([FromBody] dynamic request)
    {
        // This endpoint would send 2FA code for admins
        return Ok(new { message = "2FA-koodi lähetetty" });
    }
}
