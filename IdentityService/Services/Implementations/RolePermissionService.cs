using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;
using SharedService.Entities;

namespace IdentityService.Services.Implementations;

public class RolePermissionService : IRolePermissionService
{
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public RolePermissionService(IRolePermissionRepository rolePermissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<List<RolePermission>> GetAllPermissionsAsync()
    {
        return await _rolePermissionRepository.GetAllWithHierarchyAsync();
    }

    public async Task<RolePermission?> GetByIdAsync(Guid id)
    {
        return await _rolePermissionRepository.GetByIdAsync(id);
    }

    public async Task CreatePermissionAsync(RolePermission rolePermission)
    {
        await _rolePermissionRepository.AddAsync(rolePermission);
    }

    public async Task<List<RolePermission>> GetAncestorsAsync(Guid permissionId)
    {
        return await _rolePermissionRepository.GetAncestorsAsync(permissionId);
    }
}
