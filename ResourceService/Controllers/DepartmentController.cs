using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceService.Entities;
using ResourceService.Services.Interfaces;

namespace ResourceService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentController(IDepartmentService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(policy: "CanViewDepartments")]
    public async Task<ActionResult<List<Department>>> GetAll()
    {
        var departments = await _service.GetAllAsync();
        return Ok(departments);
    }

    [HttpGet("{id}")]
    [Authorize(policy: "CanViewDepartment")]
    public async Task<ActionResult<Department>> GetById(Guid id)
    {
        var department = await _service.GetByIdAsync(id);
        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpPost]
    [Authorize(policy: "CanCreateDepartment")]
    public async Task<ActionResult<Department>> Create(Department department)
    {
        var created = await _service.CreateAsync(department);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(policy: "CanUpdateDepartment")]
    public async Task<ActionResult<Department>> Update(Guid id, Department department)
    {
        if (id != department.Id)
        {
            return BadRequest();
        }

        var updated = await _service.UpdateAsync(department);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(policy: "CanDeleteDepartment")]
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