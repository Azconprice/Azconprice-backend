using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Models.DTOs.Profession;
using Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionController(IProfessionService professionService) : ControllerBase
    {
        private readonly IProfessionService _professionService = professionService;

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] ProfessionDTO professionDto)
        {
            try
            {
                await _professionService.AddProfessionAsync(professionDto);
                return Ok(new { Message = "Profession added successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] ProfessionUpdateDTO updateDto)
        {
            try
            {
                await _professionService.UpdateProfessionAsync(id, updateDto);
                return Ok(new { Message = "Profession updated successfully." });
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
                await _professionService.DeleteProfessionAsync(id);
                return Ok(new { Message = "Profession deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProfessionShowDTO>>> GetAll()
        {
            var result = await _professionService.GetAllProfessionsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProfessionShowDTO?>> GetById(string id)
        {
            var result = await _professionService.GetProfessionByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}