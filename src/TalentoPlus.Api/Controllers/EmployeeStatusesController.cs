using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.DTOs;

namespace TalentoPlus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeStatusesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeStatusesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/employeestatuses
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var employeeStatuses = await _unitOfWork.EmployeeStatuses.GetAllAsync();
            var dtos = employeeStatuses.Select(es => new EmployeeStatusDto
            {
                Id = es.Id,
                Name = es.Name
            });
            return Ok(dtos);
        }

        // GET: api/employeestatuses/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var employeeStatus = await _unitOfWork.EmployeeStatuses.GetByIdAsync(id);
            if (employeeStatus == null) return NotFound();
            
            var dto = new EmployeeStatusDto
            {
                Id = employeeStatus.Id,
                Name = employeeStatus.Name
            };
            return Ok(dto);
        }

        // NO hay POST, PUT, DELETE porque los estados son fijos
    }
}