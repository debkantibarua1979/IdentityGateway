using IdentityService.Dtos;
using IdentityService.Services.Interfaces;

namespace IdentityService.AuthController;

using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)
            return Unauthorized("Invalid email or password");

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var response = await _authService.RefreshAccessTokenAsync(request);
        if (response == null)
            return Unauthorized("Refresh token invalid or expired");

        return Ok(response);
    }
}
