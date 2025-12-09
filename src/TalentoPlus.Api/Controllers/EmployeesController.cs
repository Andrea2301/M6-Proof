using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.DTOs;
using TalentoPlus.Infrastructure.Interfaces;

namespace TalentoPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExcelImportService _excelImportService;

        public EmployeesController(
            IUnitOfWork unitOfWork,
            IExcelImportService excelImportService)
        {
            _unitOfWork = unitOfWork;
            _excelImportService = excelImportService;
        }

        /// <summary>
        /// Importar empleados desde un archivo Excel
        /// </summary>
        [HttpPost("import-excel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No se proporcionó ningún archivo" });
            }

            var result = await _excelImportService.ImportEmployeesAsync(file);

            if (!result.Success && result.SuccessfulImports == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors,
                    warnings = result.Warnings
                });
            }

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                data = new
                {
                    totalRows = result.TotalRows,
                    successfulImports = result.SuccessfulImports,
                    updatedRecords = result.UpdatedRecords,
                    failedImports = result.FailedImports
                },
                warnings = result.Warnings,
                errors = result.Errors
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _unitOfWork.Employees.GetAllWithDetailsAsync();
            var result = employees.Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                Document = e.Document,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                PositionName = e.Position?.Name,
                DepartmentName = e.Department?.Name,
                EmployeeStatusName = e.EmployeeStatus?.Name,
                EducationLevelName = e.EducationLevel?.Name,
                Salary = e.Salary,
                HireDate = e.HireDate,
                IsEnabled = e.IsEnabled
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _unitOfWork.Employees.GetByIdWithDetailsAsync(id);
            if (e == null) return NotFound();

            var result = new EmployeeResponseDto
            {
                Id = e.Id,
                Document = e.Document,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                PositionName = e.Position?.Name,
                DepartmentName = e.Department?.Name,
                EmployeeStatusName = e.EmployeeStatus?.Name,
                EducationLevelName = e.EducationLevel?.Name,
                Salary = e.Salary,
                HireDate = e.HireDate,
                IsEnabled = e.IsEnabled
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validar que los IDs de referencia existan
            var positionExists = await _unitOfWork.Positions.GetByIdAsync(dto.PositionId) != null;
            var departmentExists = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId) != null;
            var statusExists = await _unitOfWork.EmployeeStatuses.GetByIdAsync(dto.EmployeeStatusId) != null;
            var educationExists = await _unitOfWork.EducationLevels.GetByIdAsync(dto.EducationLevelId) != null;

            if (!positionExists || !departmentExists || !statusExists || !educationExists)
            {
                return BadRequest("Uno o más IDs de referencia no existen en la base de datos");
            }

            var employee = new Employee
            {
                Document = dto.Document,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                PositionId = dto.PositionId,
                DepartmentId = dto.DepartmentId,
                EmployeeStatusId = dto.EmployeeStatusId,
                EducationLevelId = dto.EducationLevelId,
                Salary = dto.Salary,
                HireDate = dto.HireDate,
                ProfessionalProfile = dto.ProfessionalProfile,
                IsEnabled = true,
                RegistrationDate = DateTime.UtcNow
            };

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.CompleteAsync();

            var result = new EmployeeResponseDto
            {
                Id = employee.Id,
                Document = employee.Document,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                PositionName = employee.Position?.Name,
                DepartmentName = employee.Department?.Name,
                EmployeeStatusName = employee.EmployeeStatus?.Name,
                EducationLevelName = employee.EducationLevel?.Name,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                IsEnabled = employee.IsEnabled
            };

            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null) return NotFound();

            // Validar que los IDs de referencia existan si se envían (mayores a 0)
            if (dto.PositionId > 0)
            {
                var positionExists = await _unitOfWork.Positions.GetByIdAsync(dto.PositionId) != null;
                if (!positionExists) return BadRequest("El PositionId no existe");
                employee.PositionId = dto.PositionId;
            }

            if (dto.DepartmentId > 0)
            {
                var departmentExists = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId) != null;
                if (!departmentExists) return BadRequest("El DepartmentId no existe");
                employee.DepartmentId = dto.DepartmentId;
            }

            if (dto.EmployeeStatusId > 0)
            {
                var statusExists = await _unitOfWork.EmployeeStatuses.GetByIdAsync(dto.EmployeeStatusId) != null;
                if (!statusExists) return BadRequest("El EmployeeStatusId no existe");
                employee.EmployeeStatusId = dto.EmployeeStatusId;
            }

            if (dto.EducationLevelId > 0)
            {
                var educationExists = await _unitOfWork.EducationLevels.GetByIdAsync(dto.EducationLevelId) != null;
                if (!educationExists) return BadRequest("El EducationLevelId no existe");
                employee.EducationLevelId = dto.EducationLevelId;
            }

            // Actualizar solo los campos que vienen en el DTO (si no son null/empty)
            if (!string.IsNullOrEmpty(dto.FirstName))
                employee.FirstName = dto.FirstName;
                
            if (!string.IsNullOrEmpty(dto.LastName))
                employee.LastName = dto.LastName;
                
            if (!string.IsNullOrEmpty(dto.Phone))
                employee.Phone = dto.Phone;
                
            if (!string.IsNullOrEmpty(dto.Address))
                employee.Address = dto.Address;
                
            if (!string.IsNullOrEmpty(dto.ProfessionalProfile))
                employee.ProfessionalProfile = dto.ProfessionalProfile;
                
            if (dto.Salary > 0)
                employee.Salary = dto.Salary;

            await _unitOfWork.CompleteAsync();

            var result = new EmployeeResponseDto
            {
                Id = employee.Id,
                Document = employee.Document,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                PositionName = employee.Position?.Name,
                DepartmentName = employee.Department?.Name,
                EmployeeStatusName = employee.EmployeeStatus?.Name,
                EducationLevelName = employee.EducationLevel?.Name,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                IsEnabled = employee.IsEnabled
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null) return NotFound();

            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        /// <summary>
        /// Descargar hoja de vida de un empleado en PDF
        /// </summary>
        [HttpGet("{id}/resume-pdf")]
        public async Task<IActionResult> DownloadResumePdf(int id)
        {
            try
            {
                var employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(id);
                
                if (employee == null)
                    return NotFound(new { message = "Empleado no encontrado" });

                var pdfService = HttpContext.RequestServices.GetRequiredService<IPdfService>();
                var pdfBytes = pdfService.GenerateEmployeeResume(employee);

                var fileName = $"HojaDeVida_{employee.Document}_{employee.LastName}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al generar PDF", error = ex.Message });
            }
        }
    }
}