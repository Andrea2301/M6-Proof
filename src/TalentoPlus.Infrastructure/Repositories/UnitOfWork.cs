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
        private readonly AppDbContext _context;

        private IEmployeeRepository? _employees;
        private IRepository<Position>? _positions;
        private IRepository<Department>? _departments;
        private IRepository<EducationLevel>? _educationLevels;
        private IRepository<EmployeeStatus>? _employeeStatuses;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // ---------- REPOSITORIOS ESPECÍFICOS ----------
        public IEmployeeRepository Employees =>
            _employees ??= new EmployeeRepository(_context);

        // ---------- REPOSITORIOS GENÉRICOS ----------
        public IRepository<Position> Positions =>
            _positions ??= new Repository<Position>(_context);

        public IRepository<Department> Departments =>
            _departments ??= new Repository<Department>(_context);

        public IRepository<EducationLevel> EducationLevels =>
            _educationLevels ??= new Repository<EducationLevel>(_context);

        public IRepository<EmployeeStatus> EmployeeStatuses =>
            _employeeStatuses ??= new Repository<EmployeeStatus>(_context);

        // ---------- SAVE CHANGES ----------
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // ---------- DISPOSE ----------
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}