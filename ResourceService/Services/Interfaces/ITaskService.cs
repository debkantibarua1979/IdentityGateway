namespace ResourceService.Services.Interfaces;

public interface ITaskService
{
    Task<List<Entities.Task>> GetAllAsync();
    Task<Entities.Task?> GetByIdAsync(Guid id);
    Task<Entities.Task> CreateAsync(Entities.Task task);
    Task<Entities.Task> UpdateAsync(Entities.Task task);
    Task<bool> DeleteAsync(Guid id);
}
