using SharedService.Entities;

namespace IdentityService.Repositories.Interfaces;

public interface IRolePermissionRepository
{
    Task<List<RolePermission>> GetAllPermissionsAsync();
    Task<List<RolePermission>> GetPermissionsWithAncestorsAsync(IEnumerable<Guid> permissionIds);
    Task AssignPermissionsToRoleAsync(Guid roleId, IEnumerable<Guid> permissionIds);
    Task<List<string>> GetPermissionsByUserIdAsync(Guid userId);
}
