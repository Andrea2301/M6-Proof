using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CatalogsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Listar todos los cargos (público)
        /// </summary>
        [HttpGet("positions")]
        public async Task<IActionResult> GetPositions()
        {
            var positions = await _unitOfWork.Positions.GetAllAsync();
            
            var result = positions.Select(p => new
            {
                id = p.Id,
                name = p.Name
            });

            return Ok(result);
        }

        /// <summary>
        /// Listar todos los niveles educativos (público)
        /// </summary>
        [HttpGet("education-levels")]
        public async Task<IActionResult> GetEducationLevels()
        {
            var levels = await _unitOfWork.EducationLevels.GetAllAsync();
            
            var result = levels.Select(e => new
            {
                id = e.Id,
                name = e.Name
            });

            return Ok(result);
        }

        /// <summary>
        /// Listar todos los estados de empleado (público)
        /// </summary>
        [HttpGet("employee-statuses")]
        public async Task<IActionResult> GetEmployeeStatuses()
        {
            var statuses = await _unitOfWork.EmployeeStatuses.GetAllAsync();
            
            var result = statuses.Select(s => new
            {
                id = s.Id,
                name = s.Name
            });

            return Ok(result);
        }

        /// <summary>
        /// Obtener todos los catálogos en una sola llamada (público)
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCatalogs()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var positions = await _unitOfWork.Positions.GetAllAsync();
            var educationLevels = await _unitOfWork.EducationLevels.GetAllAsync();
            var statuses = await _unitOfWork.EmployeeStatuses.GetAllAsync();

            var result = new
            {
                departments = departments.Select(d => new { id = d.Id, name = d.Name }),
                positions = positions.Select(p => new { id = p.Id, name = p.Name }),
                educationLevels = educationLevels.Select(e => new { id = e.Id, name = e.Name }),
                employeeStatuses = statuses.Select(s => new { id = s.Id, name = s.Name })
            };

            return Ok(result);
        }
    }
}