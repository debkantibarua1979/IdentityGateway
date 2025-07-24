using Microsoft.EntityFrameworkCore;
using ResourceService.Entities;
using SharedService.Entities;
using SharedService.Entities.JoinEntities;
using Task = ResourceService.Entities.Task;


namespace ResourceService.Data;

public class AppDbContext: DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RoleRolePermission> RoleRolePermissions => Set<RoleRolePermission>();
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Task>()
            .HasOne(t => t.Project) // A Task has one Project
            .WithMany(p => p.Tasks) // A Project has many Tasks
            .HasForeignKey(t => t.ProjectId); // Foreign key for ProjectId in Task
    }



}