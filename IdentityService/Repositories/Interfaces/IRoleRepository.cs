using SharedService.Entities;

namespace IdentityService.Services.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<List<Role>> GetAllAsync();
    Task AddAsync(Role role);
    Task<List<RolePermission>> GetPermissionsAsync(Guid roleId);
    Task AssignPermissionWithAncestorsAsync(Guid roleId, RolePermission permission);
    Task RemovePermissionAsync(Guid roleId, Guid rolePermissionId);
    Task<bool> HasPermissionAsync(Guid roleId, Guid rolePermissionId);
}