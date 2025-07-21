using IdentityService.Data;

namespace IdentityService.Services;

using IdentityService.Dtos;
using SharedService.Entities;



using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using SharedService.Entities;


public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return false;

        var hashedPassword = BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name,
            UserName = request.UserName,
            Email = request.Email,
            Designation = request.Designation,
            Password = hashedPassword,
            RoleId = request.RoleId
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(string accessToken, string refreshToken)?> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RolePermissionRoles)
            .ThenInclude(rpr => rpr.RolePermission)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Verify(request.Password, user.Password))
            return null;

        var accessToken = GenerateJwt(user, request.IpAddress, out var expiresAt);
        var refreshToken = Guid.NewGuid().ToString();

        _db.AccessTokens.Add(new AccessToken
        {
            UserId = user.Id,
            Token = accessToken,
            IpAddress = request.IpAddress,
            ExpiresAt = expiresAt
        });

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpireDays"]))
        });

        await _db.SaveChangesAsync();
        return (accessToken, refreshToken);
    }

    public async Task<string?> RefreshAccessTokenAsync(RefreshRequest request)
    {
        var tokenRecord = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && !t.IsRevoked);

        if (tokenRecord == null || tokenRecord.ExpiresAt < DateTime.UtcNow)
            return null;

        var user = await _db.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RolePermissionRoles)
            .ThenInclude(rpr => rpr.RolePermission)
            .FirstOrDefaultAsync(u => u.Id == tokenRecord.UserId);

        if (user == null)
            return null;

        var newToken = GenerateJwt(user, request.IpAddress, out DateTime expiresAt);

        _db.AccessTokens.Add(new AccessToken
        {
            UserId = user.Id,
            Token = newToken,
            IpAddress = request.IpAddress,
            ExpiresAt = expiresAt
        });

        await _db.SaveChangesAsync();
        return newToken;
    }

    private string GenerateJwt(User user, string ip, out DateTime expiresAt)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

        var permissions = user.Role?.RolePermissionRoles?
            .Select(rpr => rpr.RolePermission?.PermissionName)
            .Where(p => p != null)
            .ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("ip", ip)
        };

        if (permissions != null)
        {
            claims.AddRange(permissions.Select(p => new Claim("permission", p!)));
        }

        expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["AccessTokenExpireMinutes"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
