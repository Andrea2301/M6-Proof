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
