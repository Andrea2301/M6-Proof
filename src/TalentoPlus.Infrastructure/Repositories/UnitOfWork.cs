using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        private IRepository<Employee>? _employees;
        private IRepository<Position>? _positions;
        private IRepository<Department>? _departments;
        private IRepository<EducationLevel>? _educationLevels;
        private IRepository<EmployeeStatus>? _employeeStatuses;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<Employee> Employees => 
            _employees ??= new Repository<Employee>(_context);
            
        public IRepository<Position> Positions => 
            _positions ??= new Repository<Position>(_context);
            
        public IRepository<Department> Departments => 
            _departments ??= new Repository<Department>(_context);
            
        public IRepository<EducationLevel> EducationLevels => 
            _educationLevels ??= new Repository<EducationLevel>(_context);
            
        public IRepository<EmployeeStatus> EmployeeStatuses => 
            _employeeStatuses ??= new Repository<EmployeeStatus>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
