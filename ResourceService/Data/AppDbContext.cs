using Microsoft.EntityFrameworkCore;
using ResourceService.Entities;
using Task = ResourceService.Entities.Task;


namespace ResourceService.Data;

public class AppDbContext: DbContext
{
    public DbSet<Department> Departments { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Task> Tasks { get; set; }
    
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