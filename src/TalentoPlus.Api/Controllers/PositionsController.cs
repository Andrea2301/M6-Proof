using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.DTOs;

namespace TalentoPlus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PositionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/positions
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var positions = await _unitOfWork.Positions.GetAllAsync();
            var dtos = positions.Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name
            });
            return Ok(dtos);
        }

        // GET: api/positions/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var position = await _unitOfWork.Positions.GetByIdAsync(id);
            if (position == null) return NotFound();
            
            var dto = new PositionDto
            {
                Id = position.Id,
                Name = position.Name
            };
            return Ok(dto);
        }

        // POST: api/positions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PositionCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var position = new Position
            {
                Name = dto.Name
            };

            await _unitOfWork.Positions.AddAsync(position);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetById), new { id = position.Id }, position);
        }

        // PUT: api/positions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PositionUpdateDto dto)
        {
            var position = await _unitOfWork.Positions.GetByIdAsync(id);
            if (position == null) return NotFound();

            position.Name = dto.Name;
            _unitOfWork.Positions.Update(position);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // DELETE: api/positions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var position = await _unitOfWork.Positions.GetByIdAsync(id);
            if (position == null) return NotFound();

            _unitOfWork.Positions.Remove(position);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}