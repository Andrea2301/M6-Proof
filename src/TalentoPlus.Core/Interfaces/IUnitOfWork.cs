using System;
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Entities.Employee> Employees { get; }
        IRepository<Entities.Department> Departments { get; }
        IRepository<Entities.Position> Positions { get; }
        IRepository<Entities.EmployeeStatus> EmployeeStatuses { get; }
        IRepository<Entities.EducationLevel> EducationLevels { get; }
        Task<int> CompleteAsync();
    }
}
