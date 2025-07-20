using ResourceService.Repositories.Interfaces;
using ResourceService.Services.Interfaces;

namespace ResourceService.Services.Implementations;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Entities.Task>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Entities.Task?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Entities.Task> CreateAsync(Entities.Task task)
    {
        return await _repository.CreateAsync(task);
    }

    public async Task<Entities.Task> UpdateAsync(Entities.Task task)
    {
        return await _repository.UpdateAsync(task);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }
}