using SharedService.Entities;

namespace IdentityService.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByUsernameAsync(string username);

    Task<User?> GetByEmailAsync(string email);

    Task<List<User>> GetAllAsync();

    Task AddAsync(User user);

    Task<bool> ExistsByUsernameAsync(string username);

    Task<bool> ExistsByEmailAsync(string email);
}