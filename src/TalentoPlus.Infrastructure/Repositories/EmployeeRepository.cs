using Microsoft.EntityFrameworkCore;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByDocumentAsync(string document)
    {
        return await _context.Set<Employee>()
            .Include(e => e.Position)
            .Include(e => e.Department)
            .Include(e => e.EducationLevel)
            .Include(e => e.EmployeeStatus)
            .FirstOrDefaultAsync(e => e.Document == document);
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _context.Set<Employee>()
            .Include(e => e.Position)
            .Include(e => e.Department)
            .Include(e => e.EducationLevel)
            .Include(e => e.EmployeeStatus)
            .FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<Employee>> GetAllWithDetailsAsync()
    {
        return await _context.Set<Employee>()
            .Include(e => e.Position)
            .Include(e => e.Department)
            .Include(e => e.EducationLevel)
            .Include(e => e.EmployeeStatus)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Set<Employee>()
            .Include(e => e.Position)
            .Include(e => e.Department)
            .Include(e => e.EducationLevel)
            .Include(e => e.EmployeeStatus)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}