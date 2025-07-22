namespace IdentityService.Repositories.Interfaces;

public interface IRolePermissionRepository
{
    Task<List<string>> GetPermissionsByUserIdAsync(Guid userId);
}