using System.IdentityModel.Tokens.Jwt;
using System.Net;
using IdentityService.Repositories.Interfaces;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace IdentityService.Middlewares;

using JwtClaims = JwtRegisteredClaimNames;

public class TokenExpirationMiddleware : IMiddleware
{
    private readonly ITokenRepository _tokenRepo;

    public TokenExpirationMiddleware(ITokenRepository tokenRepo)
    {
        _tokenRepo = tokenRepo;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwt = handler.ReadJwtToken(token);
                var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtClaims.Sub)?.Value;
                var tokenIp = jwt.Claims.FirstOrDefault(c => c.Type == "ip")?.Value;
                var requestIp = context.Connection.RemoteIpAddress?.ToString();

                var tokenRecord = await _tokenRepo.GetAccessTokenAsync(token);

                if (tokenRecord == null ||
                    tokenRecord.ExpiresAt < DateTime.UtcNow ||
                    tokenRecord.IpAddress != requestIp ||
                    tokenRecord.UserId.ToString() != userId)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Access token is invalid, expired, or IP mismatch.");
                    return;
                }
            }
        }

        await next(context);
    }
}
