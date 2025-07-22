using SharedService.Entities;

namespace IdentityService.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid userId);
    Task CreateUserAsync(User user);
}