using System.Security.Cryptography;
using IdentityService.Data;
using IdentityService.Dtos;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace IdentityService.Services.Implementations;

using IdentityService.Dtos;
using SharedService.Entities;



using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using SharedService.Entities;


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IRolePermissionRepository _permissionRepo;
    private readonly ITokenRepository _tokenRepo;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IUserRepository userRepo,
        IRolePermissionRepository permissionRepo,
        ITokenRepository tokenRepo,
        IOptions<JwtOptions> jwtOptions)
    {
        _userRepo = userRepo;
        _permissionRepo = permissionRepo;
        _tokenRepo = tokenRepo;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<GenericResult> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepo.GetByEmailAsync(request.Email);
        if (existing != null)
            return GenericResult.Fail("Email already in use.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = BCrypt.HashPassword(request.Password),
            RoleId = request.RoleId
        };

        await _userRepo.CreateUserAsync(user);
        return GenericResult.Ok("User registered successfully.");
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Verify(request.Password, user.Password))
            return null;

        var permissions = await _permissionRepo.GetPermissionsByUserIdAsync(user.Id);

        var accessToken = GenerateAccessToken(user, permissions, request.IpAddress);
        var refreshToken = GenerateRefreshToken();

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        await _tokenRepo.SaveAccessTokenAsync(new AccessToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = accessToken,
            ExpiresAt = expiresAt,
            IpAddress = request.IpAddress
        });

        await _tokenRepo.SaveRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpireDays),
            IpAddress = request.IpAddress,
            IsRevoked = false
        });

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse?> RefreshAccessTokenAsync(RefreshTokenRequest request)
    {
        var refresh = await _tokenRepo.GetRefreshTokenAsync(request.RefreshToken);
        if (refresh == null || refresh.ExpiresAt < DateTime.UtcNow || refresh.IsRevoked)
            return null;

        var user = await _userRepo.GetByIdAsync(refresh.UserId);
        if (user == null)
            return null;

        var permissions = await _permissionRepo.GetPermissionsByUserIdAsync(user.Id);

        var newAccessToken = GenerateAccessToken(user, permissions, request.IpAddress);
        var newRefreshToken = GenerateRefreshToken();

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        await _tokenRepo.SaveAccessTokenAsync(new AccessToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newAccessToken,
            ExpiresAt = expiresAt,
            IpAddress = request.IpAddress
        });

        await _tokenRepo.SaveRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpireDays),
            IpAddress = request.IpAddress,
            IsRevoked = false
        });

        await _tokenRepo.InvalidateRefreshTokenAsync(refresh.Token);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    private string GenerateAccessToken(User user, List<string> permissions, string? ip)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("ip", ip ?? ""),
            new("role", user.RoleId.ToString())
        };

        claims.AddRange(permissions.Select(p => new Claim("permission", p)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}

