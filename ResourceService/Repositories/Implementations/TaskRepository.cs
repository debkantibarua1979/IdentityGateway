using Microsoft.EntityFrameworkCore;
using ResourceService.Data;
using ResourceService.Entities;
using ResourceService.Repositories.Interfaces;
using Task = ResourceService.Entities.Task;

namespace ResourceService.Repositories.Implementations;

public class TaskRepository: ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Entities.Task>> GetAllAsync()
    {
        return await _context.Tasks.Include(t => t.Project).ToListAsync();
    }

    public async Task<Entities.Task?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks
            .Include<Task, Project>(t => t.Project)  // Explicitly specify types for Include
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Entities.Task> CreateAsync(Entities.Task task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<Entities.Task> UpdateAsync(Entities.Task task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}
