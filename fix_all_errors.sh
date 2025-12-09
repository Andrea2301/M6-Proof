#!/bin/bash

echo "=== CORRIGIENDO TODOS LOS ERRORES ==="

# 1. INSTALAR PAQUETES NECESARIOS
echo "1. Instalando paquetes NuGet..."
cd src/TalentoPlus.Core
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package System.Linq.Expressions
cd ../..

# 2. CORREGIR User.cs
echo "2. Corrigiendo User.cs..."
cat > src/TalentoPlus.Core/Entities/User.cs << 'USER_FIX'
using System;
using Microsoft.AspNetCore.Identity;

namespace TalentoPlus.Core.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}
USER_FIX

# 3. CORREGIR IRepository.cs
echo "3. Corrigiendo IRepository.cs..."
cat > src/TalentoPlus.Core/Interfaces/IRepository.cs << 'REPO_FIX'
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
REPO_FIX

# 4. CORREGIR IUnitOfWork.cs
echo "4. Corrigiendo IUnitOfWork.cs..."
cat > src/TalentoPlus.Core/Interfaces/IUnitOfWork.cs << 'UOW_FIX'
using System;
using System.Threading.Tasks;
using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Employee> Employees { get; }
        IRepository<Department> Departments { get; }
        IRepository<Position> Positions { get; }
        IRepository<EmployeeStatus> EmployeeStatuses { get; }
        IRepository<EducationLevel> EducationLevels { get; }
        Task<int> CompleteAsync();
    }
}
UOW_FIX

# 5. CORREGIR IPdfService.cs
echo "5. Corrigiendo IPdfService.cs..."
cat > src/TalentoPlus.Core/Interfaces/IPdfService.cs << 'PDF_FIX'
using System.Threading.Tasks;
using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateResumeAsync(Employee employee);
    }
}
PDF_FIX

# 6. CORREGIR IExcelService.cs
echo "6. Corrigiendo IExcelService.cs..."
cat > src/TalentoPlus.Core/Interfaces/IExcelService.cs << 'EXCEL_FIX'
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces
{
    public interface IExcelService
    {
        Task<IEnumerable<Employee>> ReadEmployeesFromExcelAsync(Stream fileStream);
        Task<bool> ValidateExcelStructureAsync(Stream fileStream);
    }
}
EXCEL_FIX

# 7. CORREGIR IAIService.cs (si existe)
if [ -f "src/TalentoPlus.Core/Interfaces/IAIService.cs" ]; then
    echo "7. Corrigiendo IAIService.cs..."
    cat > src/TalentoPlus.Core/Interfaces/IAIService.cs << 'AI_FIX'
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IAIService
    {
        Task<string> ProcessQueryAsync(string question);
    }
}
AI_FIX
fi

# 8. CORREGIR IEmailService.cs (si existe)
if [ -f "src/TalentoPlus.Core/Interfaces/IEmailService.cs" ]; then
    echo "8. Corrigiendo IEmailService.cs..."
    cat > src/TalentoPlus.Core/Interfaces/IEmailService.cs << 'EMAIL_FIX'
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
EMAIL_FIX
fi

# 9. INSTALAR PAQUETES EN INFRASTRUCTURE
echo "9. Instalando paquetes en Infrastructure..."
cd src/TalentoPlus.Infrastructure
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package MailKit
dotnet add package QuestPDF
dotnet add package EPPlus
dotnet add package BCrypt.Net-Next
cd ../..

# 10. INSTALAR PAQUETES EN WEB
echo "10. Instalando paquetes en Web..."
cd src/TalentoPlus.Web
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.UI
cd ../..

# 11. INSTALAR PAQUETES EN API
echo "11. Instalando paquetes en API..."
cd src/TalentoPlus.Api
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
cd ../..

# 12. VERIFICAR REFERENCIAS
echo "12. Verificando referencias entre proyectos..."
cd src/TalentoPlus.Infrastructure
dotnet add reference ../TalentoPlus.Core
cd ../TalentoPlus.Web
dotnet add reference ../TalentoPlus.Infrastructure
cd ../TalentoPlus.Api
dotnet add reference ../TalentoPlus.Infrastructure
cd ../..

echo ""
echo "=== CORRECCIONES COMPLETADAS ==="
echo ""
echo "Ahora ejecuta:"
echo "1. cd src/TalentoPlus.Core"
echo "2. dotnet build"
echo ""
echo "Si sigue dando error, ejecuta:"
echo "dotnet clean && dotnet restore"
