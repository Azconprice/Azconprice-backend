using Application.Models.DTOs;
using Application.Models.DTOs.Worker;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController(IWorkerService service) : ControllerBase
    {
        private readonly IWorkerService _service = service;

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<ActionResult<WorkerProfileDTO>> GetProfile(string id)
        {
            var result = await _service.GetWorkerProfile(id);
            if (result is not null)
                return Ok(result);

            return NotFound("Worker profile not found.");
        }


        [HttpGet("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<ActionResult<WorkerProfileDTO>> GetMyProfile()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _service.GetWorkerProfile(userId);
            if (result is not null)
                return Ok(result);

            return NotFound("Worker profile not found.");
        }

        [HttpPatch("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> PatchMyProfile([FromForm] WorkerUpdateProfileDTO updateDto)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var updated = await _service.UpdateWorkerProfile(updateDto, userId);
                if (!updated)
                    return NotFound("Worker profile not found.");

                return Ok(new { Message = "Profile updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch("profile/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> PatchProfile(string id, [FromForm] WorkerUpdateProfileDTO updateDto)
        {
            try
            {
                var updated = await _service.UpdateWorkerProfile(updateDto, id);
                if (!updated)
                    return NotFound("Worker profile not found.");

                return Ok(new { Message = "Profile updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("profile/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> DeleteProfile(string id)
        {
            var deleted = await _service.DeleteWorkerProfile(id);
            if (!deleted)
                return NotFound("Worker profile not found.");

            return Ok(new { Message = "Profile deleted successfully." });
        }
    }
}
