using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;
using SharedService.Entities;

namespace IdentityService.Services.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public RoleService(
        IRoleRepository roleRepository,
        IRolePermissionRepository rolePermissionRepository)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<List<Role>> GetAllRolesAsync()
    {
        return await _roleRepository.GetAllAsync();
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _roleRepository.GetByIdAsync(id);
    }

    public async Task CreateRoleAsync(Role role)
    {
        await _roleRepository.AddAsync(role);
        // SaveChanges will be handled externally (if applicable)
    }

    public async Task<List<RolePermission>> GetPermissionsAsync(Guid roleId)
    {
        return await _roleRepository.GetPermissionsAsync(roleId);
    }

    public async Task AssignPermissionWithAncestorsAsync(Guid roleId, Guid permissionId)
    {
        var permission = await _rolePermissionRepository.GetByIdAsync(permissionId);
        if (permission == null)
            throw new Exception("Permission not found");

        await _roleRepository.AssignPermissionWithAncestorsAsync(roleId, permission);
    }

    public async Task RemovePermissionAsync(Guid roleId, Guid permissionId)
    {
        await _roleRepository.RemovePermissionAsync(roleId, permissionId);
    }
}