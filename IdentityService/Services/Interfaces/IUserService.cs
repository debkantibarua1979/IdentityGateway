using SharedService.Entities;

namespace IdentityService.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByUsernameAsync(string username);

    Task<User?> GetByEmailAsync(string email);

    Task<List<User>> GetAllAsync();

    Task CreateUserAsync(User user, string plainPassword);

    Task<bool> CheckPasswordAsync(string username, string plainPassword);

    Task<bool> ExistsByUsernameAsync(string username);

    Task<bool> ExistsByEmailAsync(string email);

    Task<List<RolePermission>> GetUserPermissionsAsync(Guid userId);
}