using Application.Models.DTOs.Specialization;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationController(ISpecializationService specializationService, IAppLogger appLogger) : ControllerBase
    {
        private readonly ISpecializationService _specializationService = specializationService;
        private readonly IAppLogger _appLogger = appLogger;

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] SpecializationDTO specializationDto)
        {
            try
            {
                await _specializationService.AddSpecializationAsync(specializationDto);

                await _appLogger.LogAsync(
                    action: "Specialization Added",
                    relatedEntityId: null,
                    userId: $"{User?.FindFirst("userId")}",
                    userName: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")}",
                    details: $"Specialization '{specializationDto.Name}' added to ProfessionId '{specializationDto.ProfessionId}'."
                );

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

                await _appLogger.LogAsync(
                    action: "Specialization Updated",
                    relatedEntityId: id,
                    userId: $"{User?.FindFirst("userId")}",
                    userName: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")}",
                    details: $"Specialization '{updateDto.Name}' updated for ProfessionId '{updateDto.ProfessionId}'."
                );

                return Ok(new { Message = "Specialization updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("filter-by-profession")]
        public async Task<ActionResult<IEnumerable<SpecializationShowDTO>>> FilterByProfession([FromQuery] string professionId)
        {
            try
            {
                var result = await _specializationService.FilterByProfessionAsync(professionId);
                if (!result.Any())
                    return NotFound("No specializations found for the specified profession.");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred while filtering specializations.", ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _specializationService.DeleteSpecializationAsync(id);

                await _appLogger.LogAsync(
                    action: "Specialization Deleted",
                    relatedEntityId: id,
                    userId: $"{User?.FindFirst("userId")}",
                    userName: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")}",
                    details: $"Specialization with ID '{id}' deleted by {User?.FindFirst("firstname")} {User?.FindFirst("lastname")}."
                );

                return Ok(new { Message = "Specialization deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SpecializationShowDTO>>> GetAll()
        {
            var result = await _specializationService.GetAllSpecializationsAsync();
            return Ok(result);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SpecializationShowDTO>>> Search([FromQuery] string query)
        {
            try
            {
                var result = await _specializationService.SearchSpecializationAsync(query);
                if (!result.Any())
                    return NotFound("No specializations found matching the query.");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred while searching for specializations.", ex.Message });
            }
        }
        [HttpGet("search-by-profession")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SpecializationShowDTO>>> Search([FromQuery] string query, string professionId)
        {
            try
            {
                var result = await _specializationService.SearchSpecializationByProfessionAsync(query, professionId);
                if (!result.Any())
                    return NotFound("No specializations found matching the query.");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred while searching for specializations.", ex.Message });
            }
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