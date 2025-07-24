using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;
using SharedService.Entities;

namespace IdentityService.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task CreateUserAsync(User user, string plainPassword)
    {
        if (await _userRepository.ExistsByUsernameAsync(user.Username))
            throw new Exception("Username already exists");

        if (await _userRepository.ExistsByEmailAsync(user.Email))
            throw new Exception("Email already exists");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

        await _userRepository.AddAsync(user);
        // SaveChanges to be handled externally
    }

    public async Task<bool> CheckPasswordAsync(string username, string plainPassword)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return false;

        return BCrypt.Net.BCrypt.Verify(plainPassword, user.PasswordHash);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _userRepository.ExistsByUsernameAsync(username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _userRepository.ExistsByEmailAsync(email);
    }

    public async Task<List<RolePermission>> GetUserPermissionsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.Role == null)
            return new List<RolePermission>();

        return user.Role.RoleRolePermissions
            .Select(rrp => rrp.RolePermission)
            .Distinct()
            .ToList();
    }
}