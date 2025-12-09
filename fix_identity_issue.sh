#!/bin/bash

echo "=== SOLUCIÓN DEFINITIVA PARA IDENTITY ==="

# 1. REMOVER User.cs problemático temporalmente
echo "1. Removiendo User.cs temporalmente..."
mv src/TalentoPlus.Core/Entities/User.cs src/TalentoPlus.Core/Entities/User.cs.backup

# 2. VERIFICAR paquetes instalados en Core
echo "2. Verificando paquetes en Core..."
cd src/TalentoPlus.Core
dotnet list package

# 3. INSTALAR paquete CORRECTO (sin AspNetCore en el nombre)
echo "3. Instalando paquete correcto..."
dotnet remove package Microsoft.AspNetCore.Identity.EntityFrameworkCore 2>/dev/null || true
dotnet add package Microsoft.Extensions.Identity.Stores

# 4. CREAR User.cs SIMPLIFICADO (sin Identity por ahora)
echo "4. Creando User.cs simplificado..."
cat > Entities/User.cs << 'USER_SIMPLE'
using System;

namespace TalentoPlus.Core.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}
USER_SIMPLE

# 5. CORREGIR interfaces que usan Employee
echo "5. Corrigiendo interfaces..."

# IPdfService.cs
cat > Interfaces/IPdfService.cs << 'IPDF_FIX'
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateResumeAsync(Entities.Employee employee);
    }
}
IPDF_FIX

# IExcelService.cs
cat > Interfaces/IExcelService.cs << 'IEXCEL_FIX'
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TalentoPlus.Core.Interfaces
{
    public interface IExcelService
    {
        Task<IEnumerable<Entities.Employee>> ReadEmployeesFromExcelAsync(Stream fileStream);
        Task<bool> ValidateExcelStructureAsync(Stream fileStream);
    }
}
IEXCEL_FIX

# IUnitOfWork.cs
cat > Interfaces/IUnitOfWork.cs << 'IUOW_FIX'
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
IUOW_FIX

cd ../..

# 6. PROBAR compilación
echo "6. Probando compilación..."
cd src/TalentoPlus.Core
if dotnet build; then
    echo "✓ Core compila correctamente!"
    
    # Restaurar User.cs original pero corregido
    echo "7. Creando User.cs final..."
    cat > Entities/User.cs << 'USER_FINAL'
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
USER_FINAL
    
    # Instalar paquete completo ahora
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
    
else
    echo "✗ Aún hay errores. Usando solución alternativa..."
    
    # Crear User alternativo
    cat > Entities/User.cs << 'USER_ALT'
namespace TalentoPlus.Core.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public System.DateTime RegistrationDate { get; set; } = System.DateTime.UtcNow;
    }
}
USER_ALT
fi

echo ""
echo "=== SOLUCIÓN APLICADA ==="
echo "Ejecuta: dotnet build"
