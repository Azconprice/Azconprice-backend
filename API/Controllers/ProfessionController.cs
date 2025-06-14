using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Models.DTOs.Profession;
using Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionController(IProfessionService professionService, IAppLogger appLogger) : ControllerBase
    {
        private readonly IProfessionService _professionService = professionService;
        private readonly IAppLogger _appLogger = appLogger;

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] ProfessionDTO professionDto)
        {
            try
            {
                await _professionService.AddProfessionAsync(professionDto);

                await _appLogger.LogAsync(
                    action: "Profession Added",
                    relatedEntityId: null,
                    userId: $"{User?.FindFirst("userId")}",
                    userName: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")}",
                    details: $"Profession '{professionDto.Name}' added."
                );

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

                await _appLogger.LogAsync(
                    action: "Profession Updated",
                    relatedEntityId: id,
                   userId: $"{User?.FindFirst("userId")}",
                    userName: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")}",
                    details: $"Profession '{updateDto.Name}' updated."
                );

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

                await _appLogger.LogAsync(
                    action: "Profession Deleted",
                    relatedEntityId: id, userId: $"{User?.FindFirst("userId")}",
                    userName: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")}",
                    details: $"Profession with ID '{id}' deleted."
                );

                return Ok(new { Message = "Profession deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("all")]
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