using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Models.DTOs.Specialization;
using Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationController(ISpecializationService specializationService) : ControllerBase
    {
        private readonly ISpecializationService _specializationService = specializationService;

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] SpecializationDTO specializationDto)
        {
            try
            {
                await _specializationService.AddSpecializationAsync(specializationDto);
                return Ok(new { Message = "Specialization added successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] SpecializationUpdateDTO updateDto)
        {
            try
            {
                await _specializationService.UpdateSpecializationAsync(id, updateDto);
                return Ok(new { Message = "Specialization updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _specializationService.DeleteSpecializationAsync(id);
                return Ok(new { Message = "Specialization deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SpecializationShowDTO>>> GetAll()
        {
            var result = await _specializationService.GetAllSpecializationsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<SpecializationShowDTO?>> GetById(string id)
        {
            var result = await _specializationService.GetSpecializationByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}