using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedService.Entities;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolePermissionController : ControllerBase
{
    private readonly IRolePermissionService _rolePermissionService;

    public RolePermissionController(IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPermissions()
    {
        var permissions = await _rolePermissionService.GetAllPermissionsAsync();
        return Ok(permissions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPermissionById(Guid id)
    {
        var permission = await _rolePermissionService.GetByIdAsync(id);
        if (permission == null)
            return NotFound();
        return Ok(permission);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePermission([FromBody] RolePermission permission)
    {
        await _rolePermissionService.CreatePermissionAsync(permission);
        return CreatedAtAction(nameof(GetPermissionById), new { id = permission.Id }, permission);
    }

    [HttpGet("{id:guid}/ancestors")]
    public async Task<IActionResult> GetAncestors(Guid id)
    {
        var ancestors = await _rolePermissionService.GetAncestorsAsync(id);
        return Ok(ancestors);
    }
}