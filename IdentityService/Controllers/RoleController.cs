using IdentityService.Services.Interfaces;

namespace IdentityService.Controllers;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("{roleId:guid}/assign-permission/{permissionId:guid}")]
    public async Task<IActionResult> AssignPermissionToRole(Guid roleId, Guid permissionId)
    {
        await _roleService.AssignPermissionWithAncestorsAsync(roleId, permissionId);
        return Ok(new { Message = "Permission (with ancestors) assigned to role." });
    }

    [HttpGet("{roleId:guid}/permissions")]
    public async Task<IActionResult> GetRolePermissions(Guid roleId)
    {
        var permissions = await _roleService.GetPermissionsAsync(roleId);
        return Ok(permissions);
    }

    [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}")]
    public async Task<IActionResult> RemovePermissionFromRole(Guid roleId, Guid permissionId)
    {
        await _roleService.RemovePermissionAsync(roleId, permissionId);
        return Ok(new { Message = "Permission removed from role." });
    }
}
