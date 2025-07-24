using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Dtos;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using SharedService.Entities;

namespace UserManagement.Application.Services.Impl;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtOptions _jwtOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        IUserService userService,
        IRefreshTokenRepository refreshTokenRepository,
        JwtOptions jwtOptions,
        IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtOptions = jwtOptions;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userService.GetByEmailAsync(request.Email);
        if (user == null)
            return null;

        var isValid = await _userService.CheckPasswordAsync(user.Username, request.Password);
        if (!isValid)
            return null;

        var permissions = await _userService.GetUserPermissionsAsync(user.Id);

        string ipAddress = GetCurrentIpAddress();

        var accessToken = GenerateJwtToken(user, permissions, ipAddress);
        var refreshToken = Guid.NewGuid().ToString();

        await _refreshTokenRepository.StoreAsync(user.Id, refreshToken, ipAddress, _jwtOptions.RefreshTokenExpireDays);

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Permissions = permissions.ConvertAll(p => p.Value)
        };
    }

    public async Task<AuthResponse?> RefreshAccessTokenAsync(RefreshTokenRequest request)
    {
        var userId = ValidateJwtAndGetUserId(request.RefreshToken);
        if (userId == null)
            return null;

        var isValidRefresh = await _refreshTokenRepository.ValidateAsync(userId.Value, request.RefreshToken);
        if (!isValidRefresh)
            return null;

        var user = await _userService.GetByIdAsync(userId.Value);
        if (user == null)
            return null;

        var permissions = await _userService.GetUserPermissionsAsync(user.Id);
        var ipAddress = GetCurrentIpAddress();

        var newAccessToken = GenerateJwtToken(user, permissions, ipAddress);
        var newRefreshToken = Guid.NewGuid().ToString();

        await _refreshTokenRepository.StoreAsync(user.Id, newRefreshToken, ipAddress, _jwtOptions.RefreshTokenExpireDays);

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Permissions = permissions.ConvertAll(p => p.Value)
        };
    }

    public async Task<GenericResult> RegisterAsync(RegisterRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.UserName,
            Email = request.Email,
            RoleId = request.RoleId
        };

        try
        {
            await _userService.CreateUserAsync(user, request.Password);
            return GenericResult.Ok("User registered successfully.");
        }
        catch (Exception ex)
        {
            return GenericResult.Fail(ex.Message);
        }
    }

    private string GenerateJwtToken(User user, List<RolePermission> permissions, string ipAddress)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("ip", ipAddress)
        };

        foreach (var p in permissions)
        {
            claims.Add(new Claim("permission", p.Value));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
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

    private Guid? ValidateJwtAndGetUserId(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var sub = jwtToken?.Subject;

        return Guid.TryParse(sub, out var id) ? id : null;
    }

    private string GetCurrentIpAddress()
    {
        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}


