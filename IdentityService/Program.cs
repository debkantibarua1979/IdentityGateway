using System.Text;
using IdentityService.Data;
using IdentityService.Dtos;
using IdentityService.Middlewares;
using IdentityService.Repositories.Implementations;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Implementations;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Responder;
using UserManagement.Application.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

// Load JwtOptions
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("JwtOptions"));
var jwtOptions = builder.Configuration
    .GetSection("JwtOptions").Get<JwtOptions>();

var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

// DbContext (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// Register IHttpContextAccessor for IP capture
builder.Services.AddHttpContextAccessor();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// TokenExpirationMiddleware registered as IMiddleware
builder.Services.AddTransient<TokenExpirationMiddleware>();

// Ocelot safe responder
builder.Services.AddSingleton<IHttpResponder, SafeHttpResponder>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Authorization
builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity API", Version = "v1" });
});

// Ocelot config
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// Routing
app.UseRouting();

// Auth + Middleware
app.UseAuthentication();
app.UseMiddleware<TokenExpirationMiddleware>();
app.UseAuthorization();

// API
app.MapControllers();

// Ocelot gateway
await app.UseOcelot();

app.Run();
