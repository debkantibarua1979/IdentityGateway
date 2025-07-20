using Microsoft.EntityFrameworkCore;
using ResourceService.Data;
using ResourceService.Entities;
using ResourceService.Repositories.Interfaces;

namespace ResourceService.Repositories.Implementations;

public class DepartmentRepository : IDepartmentRepository
    {
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetAllAsync()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        return await _context.Departments.FindAsync(id);
    }

    public async Task<Department> CreateAsync(Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<Department> UpdateAsync(Department department)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return false;

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return true;
    }
}
