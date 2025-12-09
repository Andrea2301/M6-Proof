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
