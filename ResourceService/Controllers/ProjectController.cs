using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceService.Entities;
using ResourceService.Services.Interfaces;

namespace ResourceService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectController(IProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(policy:"CanViewProjects")]
    public async Task<ActionResult<List<Project>>> GetAll()
    {
        var projects = await _service.GetAllAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    [Authorize(policy:"CanViewProject")]
    public async Task<ActionResult<Project>> GetById(Guid id)
    {
        var project = await _service.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpPost]
    [Authorize(policy:"CanCreateProject")]
    public async Task<ActionResult<Project>> Create(Project project)
    {
        var created = await _service.CreateAsync(project);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(policy:"CanUpdateProject")]
    public async Task<ActionResult<Project>> Update(Guid id, Project project)
    {
        if (id != project.Id)
        {
            return BadRequest();
        }

        var updated = await _service.UpdateAsync(project);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(policy:"CanDeleteProject")]
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
