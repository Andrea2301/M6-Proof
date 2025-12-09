#!/bin/bash
echo "=== REPARACIÓN COMPLETA API ==="

# 1. Limpiar
echo "1. Limpiando..."
rm -rf bin obj

# 2. Verificar y crear DTOs en Core
echo "2. Creando DTOs en TalentoPlus.Core..."
cd ../TalentoPlus.Core
mkdir -p DTOs

cat > DTOs/AuthDtos.cs << 'DTO_EOF'
using System;
using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Core.DTOs
{
    public class LoginDto
    {
        [Required]
        public string Document { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required]
        public string Document { get; set; } = string.Empty;
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string? Phone { get; set; }
        
        public string? Address { get; set; }
        
        public DateTime? BirthDate { get; set; }
        
        public decimal Salary { get; set; }
        
        public string? ProfessionalProfile { get; set; }
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
DTO_EOF

# 3. Volver a API y corregir controllers
echo "3. Corrigiendo controllers..."
cd ../TalentoPlus.Api

# Corregir AuthController
cat > Controllers/AuthController.cs << 'AUTH_EOF'
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

            var existingEmployee = await _unitOfWork.Employees.GetByDocumentAsync(registerDto.Document);
            if (existingEmployee != null)
                return BadRequest(new { message = "El documento ya está registrado" });

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
AUTH_EOF

# Corregir DepartmentsController
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
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();
            return Ok(departments);
        }
    }
}
DEPT_EOF

# Corregir EmployeesController
cat > Controllers/EmployeesController.cs << 'EMP_EOF'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPdfService _pdfService;

        public EmployeesController(IUnitOfWork unitOfWork, IPdfService pdfService)
        {
            _unitOfWork = unitOfWork;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                return NotFound();
            
            return Ok(employee);
        }
    }
}
EMP_EOF

# 4. Corregir csproj
echo "4. Corrigiendo archivo .csproj..."
cat > TalentoPlus.Api.csproj << 'PROJ_EOF'
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
PROJ_EOF

# 5. Agregar paquetes necesarios
echo "5. Agregando paquetes NuGet..."
dotnet add package Microsoft.AspNetCore.Mvc.Core --version 2.2.5
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.0.0
dotnet add package BCrypt.Net-Next --version 4.0.3
dotnet add package Swashbuckle.AspNetCore --version 6.5.0

# 6. Restaurar y construir
echo "6. Restaurando paquetes..."
dotnet restore

echo "7. Construyendo..."
dotnet build

echo ""
echo "✅ REPARACIÓN COMPLETADA"
echo ""
echo "Ejecuta: dotnet run"
echo "La API estará en:"
echo "  http://localhost:5001"
echo "  http://localhost:5001/swagger"
echo ""
echo "Endpoints disponibles:"
echo "  GET  /api/Departments"
echo "  POST /api/Auth/login"
echo "  POST /api/Auth/register"
echo "  GET  /api/Employees (requiere autenticación)"
