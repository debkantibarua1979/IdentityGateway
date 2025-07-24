using IdentityService.Dtos;
using Microsoft.EntityFrameworkCore;
using SharedService.Entities;
using SharedService.Entities.JoinEntities;

namespace IdentityService.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // User & Role management
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<RoleRolePermission> RoleRolePermissions { get; set; }

    // Token tracking
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Role-Permission join key
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoleRolePermission>()
            .HasKey(rrp => new { rrp.RoleId, rrp.RolePermissionId });

        modelBuilder.Entity<RoleRolePermission>()
            .HasOne(rrp => rrp.Role)
            .WithMany(r => r.RoleRolePermissions)
            .HasForeignKey(rrp => rrp.RoleId);

        modelBuilder.Entity<RoleRolePermission>()
            .HasOne(rrp => rrp.RolePermission)
            .WithMany(rp => rp.RoleRolePermissions)
            .HasForeignKey(rrp => rrp.RolePermissionId);
        
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Parent)
            .WithMany(rp => rp.Children)
            .HasForeignKey(rp => rp.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Token uniqueness
        modelBuilder.Entity<AccessToken>()
            .HasIndex(t => t.Token)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(t => t.Token)
            .IsUnique();
    }
}