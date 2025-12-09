#!/bin/bash

echo "=== SIMPLIFICANDO CORE ==="

# 1. ELIMINAR User.cs problemático
rm -f src/TalentoPlus.Core/Entities/User.cs

# 2. CREAR archivos CORRECTOS sin dependencias complejas
cd src/TalentoPlus.Core

# 3. Crear Employee.cs (ya existe, verificar)
if [ ! -f "Entities/Employee.cs" ]; then
    cat > Entities/Employee.cs << 'EMP'
using System;

namespace TalentoPlus.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Document { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int PositionId { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public int EmployeeStatusId { get; set; }
        public int EducationLevelId { get; set; }
        public string ProfessionalProfile { get; set; }
        public int DepartmentId { get; set; }
        public string PasswordHash { get; set; }
        public bool IsEnabled { get; set; } = false;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}
EMP
fi

# 4. Crear Department.cs
cat > Entities/Department.cs << 'DEPT'
namespace TalentoPlus.Core.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
DEPT

# 5. Crear Position.cs
cat > Entities/Position.cs << 'POS'
namespace TalentoPlus.Core.Entities
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
POS

# 6. Crear EmployeeStatus.cs
cat > Entities/EmployeeStatus.cs << 'STATUS'
namespace TalentoPlus.Core.Entities
{
    public class EmployeeStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
STATUS

# 7. Crear EducationLevel.cs
cat > Entities/EducationLevel.cs << 'EDU'
namespace TalentoPlus.Core.Entities
{
    public class EducationLevel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
EDU

# 8. CORREGIR interfaces
mkdir -p Interfaces

# IRepository.cs
cat > Interfaces/IRepository.cs << 'IREPO'
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<int> CountAsync();
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
IREPO

# IUnitOfWork.cs (SIN Employee en la firma - lo arreglamos después)
cat > Interfaces/IUnitOfWork.cs << 'IUOW'
using System;
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CompleteAsync();
    }
}
IUOW

# IPdfService.cs
cat > Interfaces/IPdfService.cs << 'IPDF'
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateResumeAsync(Employee employee);
    }
}
IPDF

# IExcelService.cs
cat > Interfaces/IExcelService.cs << 'IEXCEL'
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IExcelService
    {
        Task<IEnumerable<Employee>> ReadEmployeesFromExcelAsync(Stream fileStream);
        Task<bool> ValidateExcelStructureAsync(Stream fileStream);
    }
}
IEXCEL

# IAIService.cs
cat > Interfaces/IAIService.cs << 'IAI'
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IAIService
    {
        Task<string> ProcessQueryAsync(string question);
    }
}
IAI

# IEmailService.cs
cat > Interfaces/IEmailService.cs << 'IEMAIL'
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
IEMAIL

cd ../..

echo "✓ Core simplificado creado"
echo ""
echo "Ahora prueba:"
echo "cd src/TalentoPlus.Core && dotnet build"
