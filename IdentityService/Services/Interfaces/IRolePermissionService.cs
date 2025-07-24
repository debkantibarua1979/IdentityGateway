using SharedService.Entities;

namespace IdentityService.Services.Interfaces;

public interface IRolePermissionService
{
    Task<List<RolePermission>> GetAllPermissionsAsync();

    Task<RolePermission?> GetByIdAsync(Guid id);

    Task CreatePermissionAsync(RolePermission rolePermission);
    
    Task<List<RolePermission>> GetAncestorsAsync(Guid permissionId);
}