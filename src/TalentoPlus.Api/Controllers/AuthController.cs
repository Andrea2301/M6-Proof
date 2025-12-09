using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.DTOs;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public AuthController(
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        /// <summary>
        /// Autoregistro de empleado (público)
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verificar si el documento ya existe
                var existingEmployee = await _unitOfWork.Employees.GetByDocumentAsync(dto.Document);
                if (existingEmployee != null)
                {
                    return BadRequest(new { message = "Ya existe un empleado con este documento" });
                }

                // Verificar si el email ya existe
                var existingEmail = await _unitOfWork.Employees.GetByEmailAsync(dto.Email);
                if (existingEmail != null)
                {
                    return BadRequest(new { message = "Ya existe un empleado con este email" });
                }

                // Validar que existan los IDs de referencia
                var departmentExists = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);
                var positionExists = await _unitOfWork.Positions.GetByIdAsync(dto.PositionId);
                var educationExists = await _unitOfWork.EducationLevels.GetByIdAsync(dto.EducationLevelId);

                if (departmentExists == null || positionExists == null || educationExists == null)
                {
                    return BadRequest(new { message = "Uno o más datos de referencia son inválidos" });
                }

                // Crear el nuevo empleado
                var employee = new Employee
                {
                    Document = dto.Document,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Address = dto.Address ?? string.Empty,
                    BirthDate = dto.BirthDate,
                    DepartmentId = dto.DepartmentId,
                    PositionId = dto.PositionId,
                    EducationLevelId = dto.EducationLevelId,
                    ProfessionalProfile = dto.ProfessionalProfile ?? string.Empty,
                    EmployeeStatusId = 2, // Inactivo por defecto
                    IsEnabled = false, // Deshabilitado hasta que admin lo habilite
                    RegistrationDate = DateTime.UtcNow,
                    HireDate = DateTime.UtcNow,
                    Salary = 0 // Será asignado por el admin
                };

                await _unitOfWork.Employees.AddAsync(employee);
                await _unitOfWork.CompleteAsync();

                // Enviar correo de bienvenida
                var emailSent = await _emailService.SendWelcomeEmailAsync(
                    employee.Email, 
                    $"{employee.FirstName} {employee.LastName}"
                );

                if (!emailSent)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Registro exitoso, pero no se pudo enviar el correo de bienvenida",
                        data = new
                        {
                            id = employee.Id,
                            document = employee.Document,
                            fullName = $"{employee.FirstName} {employee.LastName}",
                            email = employee.Email,
                            emailSent = false
                        }
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Registro exitoso. Se ha enviado un correo de confirmación a tu email",
                    data = new
                    {
                        id = employee.Id,
                        document = employee.Document,
                        fullName = $"{employee.FirstName} {employee.LastName}",
                        email = employee.Email,
                        emailSent = true
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al procesar el registro",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Login de empleado (público) - próximamente con JWT
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // TODO: Implementar en la siguiente fase con JWT
            return Ok(new { message = "Login endpoint - próximamente" });
        }
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "El documento o email es requerido")]
        public string DocumentOrEmail { get; set; } = string.Empty;
    }
}