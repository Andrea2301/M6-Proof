using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    
    Task<Employee?> GetByDocumentAsync(string document);
    Task<Employee?> GetByEmailAsync(string email);
    Task<IEnumerable<Employee>> GetAllWithDetailsAsync();
    Task<Employee?> GetByIdWithDetailsAsync(int id);
}