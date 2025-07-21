using IdentityService.Dtos;

namespace IdentityService.AuthController;

using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var success = await _authService.RegisterAsync(request);
        if (!success)
            return BadRequest("User already exists.");
        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var tokens = await _authService.LoginAsync(request);
        if (tokens == null)
            return Unauthorized("Invalid credentials.");

        return Ok(new
        {
            accessToken = tokens.Value.accessToken,
            refreshToken = tokens.Value.refreshToken
        });
    }
}
