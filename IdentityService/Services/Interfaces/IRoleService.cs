using SharedService.Entities;

namespace IdentityService.Services.Interfaces;

public interface IRoleService
{
    Task<List<Role>> GetAllRolesAsync();

    Task<Role?> GetByIdAsync(Guid id);

    Task CreateRoleAsync(Role role);

    Task<List<RolePermission>> GetPermissionsAsync(Guid roleId);

    Task AssignPermissionWithAncestorsAsync(Guid roleId, Guid permissionId);

    Task RemovePermissionAsync(Guid roleId, Guid permissionId);
}