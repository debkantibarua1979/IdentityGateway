using IdentityService.Dtos;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedService.Entities;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var user = await _userService.GetByUsernameAsync(username);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            RoleId = request.RoleId
        };

        await _userService.CreateUserAsync(user, request.Password);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPost("check-password")]
    public async Task<IActionResult> CheckPassword([FromBody] CheckPasswordRequest request)
    {
        var valid = await _userService.CheckPasswordAsync(request.Username, request.Password);
        return Ok(new { IsValid = valid });
    }

    [HttpGet("{id:guid}/permissions")]
    public async Task<IActionResult> GetUserPermissions(Guid id)
    {
        var permissions = await _userService.GetUserPermissionsAsync(id);
        return Ok(permissions);
    }
}