using Amazon.Runtime.Internal;
using Application.Models.DTOs;
using Application.Models.DTOs.Worker;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController(IWorkerService service, IValidator<WorkerUpdateProfileDTO> validator) : ControllerBase
    {
        private readonly IWorkerService _service = service;
        private readonly IValidator<WorkerUpdateProfileDTO> _validator = validator;

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
            var validationResult = await _validator.ValidateAsync(updateDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Errors = errors });
            }

            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var updated = await _service.UpdateWorkerProfile(
                    userId,
                    updateDto,
                    (email, token) => Url.Action("ConfirmEmail", "Auth", new { email, token }, Request.Scheme) ?? string.Empty
                );
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
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> PatchProfile(string id, [FromForm] WorkerUpdateProfileDTO updateDto)
        {
            var validationResult = await _validator.ValidateAsync(updateDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Errors = errors });
            }

            try
            {
                var updated = await _service.UpdateWorkerProfile(
                    id,
                    updateDto,
                    (email, token) => Url.Action("ConfirmEmail", "Auth", new { email, token }, Request.Scheme) ?? string.Empty // Ensure non-null return
                );
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
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> DeleteProfile(string id)
        {
            var deleted = await _service.DeleteWorkerProfile(id);
            if (!deleted)
                return NotFound("Worker profile not found.");

            return Ok(new { Message = "Profile deleted successfully." });
        }

        [HttpDelete("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> DeleteMyProfile(string id)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var deleted = await _service.DeleteWorkerProfile(userId);
            if (!deleted)
                return NotFound("Worker profile not found.");

            return Ok(new { Message = "Profile deleted successfully." });
        }
    }
}
