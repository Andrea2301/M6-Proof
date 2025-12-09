#!/bin/bash
echo "=== REPARANDO TALENTOPLUS.API ==="

# 1. Limpiar
echo "1. Limpiando..."
rm -rf bin obj

# 2. Agregar paquetes necesarios
echo "2. Agregando paquetes NuGet..."
dotnet add package Microsoft.AspNetCore.Mvc.Core
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package BCrypt.Net-Next
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools

# 3. Verificar y corregir el .csproj
echo "3. Verificando archivo .csproj..."
cat > TalentoPlus.Api.csproj << 'CSHARP'
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Core" Version="2.2.5" />
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
CSHARP

# 4. Restaurar
echo "4. Restaurando paquetes..."
dotnet restore

# 5. Corregir namespaces en AuthController.cs
echo "5. Corrigiendo AuthController.cs..."
if [ -f "Controllers/AuthController.cs" ]; then
    cat > Controllers/AuthController.cs << 'CSHARP_CONTROLLER'
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Core.DTOs;

namespace TalentoPlus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(IUnitOfWork unitOfWork, IEmailService emailService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _unitOfWork.Employees.GetByDocumentAsync(loginDto.Document);
            
            if (employee == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, employee.PasswordHash))
                return Unauthorized(new { message = "Credenciales inválidas" });

            if (!employee.IsEnabled)
                return Unauthorized(new { message = "Cuenta deshabilitada" });

            var token = GenerateJwtToken(employee);
            
            return Ok(new AuthResponseDto
            {
                Token = token,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Email = employee.Email,
                Document = employee.Document,
                Role = employee.Position?.Name ?? "Employee"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar si el documento ya existe
            var existingEmployee = await _unitOfWork.Employees.GetByDocumentAsync(registerDto.Document);
            if (existingEmployee != null)
                return BadRequest(new { message = "El documento ya está registrado" });

            // Buscar posición por defecto
            var position = (await _unitOfWork.Positions.GetAllAsync()).FirstOrDefault();
            var department = (await _unitOfWork.Departments.GetAllAsync()).FirstOrDefault();
            var status = (await _unitOfWork.EmployeeStatuses.GetAllAsync()).FirstOrDefault(s => s.Name == "Activo");
            var education = (await _unitOfWork.EducationLevels.GetAllAsync()).FirstOrDefault();

            var employee = new Employee
            {
                Document = registerDto.Document,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                BirthDate = registerDto.BirthDate,
                PositionId = position?.Id ?? 1,
                DepartmentId = department?.Id ?? 1,
                EmployeeStatusId = status?.Id ?? 1,
                EducationLevelId = education?.Id ?? 1,
                Salary = registerDto.Salary,
                HireDate = DateTime.UtcNow,
                ProfessionalProfile = registerDto.ProfessionalProfile ?? "Nuevo empleado",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                IsEnabled = true,
                RegistrationDate = DateTime.UtcNow
            };

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.CompleteAsync();

            var token = GenerateJwtToken(employee);

            // Enviar email de bienvenida
            await _emailService.SendWelcomeEmailAsync(employee.Email, employee.FirstName);

            return Ok(new AuthResponseDto
            {
                Token = token,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Email = employee.Email,
                Document = employee.Document,
                Role = position?.Name ?? "Employee"
            });
        }

        private string GenerateJwtToken(Employee employee)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "default_secret_key_32_chars_long!"));
            
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{employee.FirstName} {employee.LastName}"),
                new Claim(ClaimTypes.Email, employee.Email ?? ""),
                new Claim("document", employee.Document)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "TalentoPlus",
                audience: _configuration["Jwt:Audience"] ?? "TalentoPlusClients",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
CSHARP_CONTROLLER
fi

# 6. Construir
echo "6. Construyendo..."
dotnet build

echo ""
echo "✅ Reparación completada"
echo ""
echo "Ejecuta: dotnet run"
echo "La API estará en: http://localhost:5001"
