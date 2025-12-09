using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Listar todos los departamentos (público)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();
            
            var result = departments.Select(d => new
            {
                id = d.Id,
                name = d.Name,
                description = d.Description
            });

            return Ok(result);
        }

        /// <summary>
        /// Obtener departamento por ID (público)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            
            if (department == null)
                return NotFound(new { message = "Departamento no encontrado" });

            var result = new
            {
                id = department.Id,
                name = department.Name,
                description = department.Description
            };

            return Ok(result);
        }
    }
}