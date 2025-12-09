using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.DTOs;

namespace TalentoPlus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationLevelsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EducationLevelsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/educationlevels
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var educationLevels = await _unitOfWork.EducationLevels.GetAllAsync();
            var dtos = educationLevels.Select(el => new EducationLevelDto
            {
                Id = el.Id,
                Name = el.Name
            });
            return Ok(dtos);
        }

        // GET: api/educationlevels/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var educationLevel = await _unitOfWork.EducationLevels.GetByIdAsync(id);
            if (educationLevel == null) return NotFound();
            
            var dto = new EducationLevelDto
            {
                Id = educationLevel.Id,
                Name = educationLevel.Name
            };
            return Ok(dto);
        }

        // POST: api/educationlevels
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EducationLevelCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var educationLevel = new EducationLevel
            {
                Name = dto.Name
            };

            await _unitOfWork.EducationLevels.AddAsync(educationLevel);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetById), new { id = educationLevel.Id }, educationLevel);
        }

        // PUT: api/educationlevels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EducationLevelUpdateDto dto)
        {
            var educationLevel = await _unitOfWork.EducationLevels.GetByIdAsync(id);
            if (educationLevel == null) return NotFound();

            educationLevel.Name = dto.Name;
            _unitOfWork.EducationLevels.Update(educationLevel);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // DELETE: api/educationlevels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var educationLevel = await _unitOfWork.EducationLevels.GetByIdAsync(id);
            if (educationLevel == null) return NotFound();

            _unitOfWork.EducationLevels.Remove(educationLevel);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}