using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceService.Services.Interfaces;

namespace ResourceService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ITaskService _service;

    public TaskController(ITaskService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(policy:"CanViewTasks")]
    public async Task<ActionResult<List<Task>>> GetAll()
    {
        var tasks = await _service.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    [Authorize(policy: "CanViewTask")]
    public async Task<ActionResult<Task>> GetById(Guid id)
    {
        var task = await _service.GetByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    [Authorize(policy: "CanCreateTask")]
    public async Task<ActionResult<Task>> Create(Entities.Task task)
    {
        var created = await _service.CreateAsync(task);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(policy: "CanUpdateTask")]
    public async Task<ActionResult<Task>> Update(Guid id, Entities.Task task)
    {
        if (id != task.Id)
        {
            return BadRequest();
        }

        var updated = await _service.UpdateAsync(task);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(policy: "CanDeleteTask")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}