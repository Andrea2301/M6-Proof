#!/bin/bash
echo "=== INSTALANDO PAQUETES FALTANTES ==="

# 1. Instalar paquetes necesarios
echo "1. Instalando Microsoft.AspNetCore.Authorization..."
dotnet add package Microsoft.AspNetCore.Authorization --version 8.0.0

echo "2. Instalando Microsoft.AspNetCore.Mvc..."
dotnet add package Microsoft.AspNetCore.Mvc --version 2.2.0

echo "3. Verificando todos los paquetes..."
cat > TalentoPlus.Api.csproj << 'PROJ_EOF'
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc" Version="2.2.0" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.0" />
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../TalentoPlus.Core/TalentoPlus.Core.csproj" />
    <ProjectReference Include="../TalentoPlus.Infrastructure/TalentoPlus.Infrastructure.csproj" />
  </ItemGroup>

</Project>
PROJ_EOF

# 4. Corregir el DepartmentsController (quitar AllowAnonymous temporalmente)
echo "4. Corrigiendo DepartmentsController..."
cat > Controllers/DepartmentsController.cs << 'DEPT_EOF'
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();
            return Ok(departments);
        }
    }
}
DEPT_EOF

# 5. Restaurar y construir
echo "5. Restaurando paquetes..."
dotnet restore

echo "6. Construyendo..."
dotnet build

echo ""
echo "✅ PAQUETES INSTALADOS"
echo ""
echo "Ejecuta: dotnet run"
