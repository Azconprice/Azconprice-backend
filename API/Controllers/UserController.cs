using Application.Models.DTOs.User;
using Application.Models.DTOs.Worker;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IClientService service,IValidator<UserUpdateDTO> validator) : Controller
    {
        private readonly IClientService _service = service;
        private readonly IValidator<UserUpdateDTO> _validator = validator;

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        public async Task<ActionResult<WorkerProfileDTO>> GetProfile(string id)
        {
            var result = await _service.GetUserByIdAsync(id);
            if (result is not null)
                return Ok(result);

            return NotFound("User profile not found.");
        }


        [HttpGet("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        public async Task<ActionResult<WorkerProfileDTO>> GetMyProfile()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _service.GetUserByIdAsync(userId);
            if (result is not null)
                return Ok(result);

            return NotFound("User profile not found.");
        }

        [HttpPatch("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        public async Task<IActionResult> PatchMyProfile([FromForm] UserUpdateDTO updateDto)
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
                var updated = await _service.UpdateUserAsync(userId, updateDto);
                if (!updated)
                    return NotFound("User profile not found.");

                return Ok(new { Message = "Profile updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch("profile/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        public async Task<IActionResult> PatchProfile(string id, [FromForm] UserUpdateDTO updateDto)
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
                var updated = await _service.UpdateUserAsync(id, updateDto);
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
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        public async Task<IActionResult> DeleteProfile(string id)
        {
            var deleted = await _service.DeleteUserAsync(id);
            if (!deleted)
                return NotFound("User profile not found.");

            return Ok(new { Message = "Profile deleted successfully." });
        }

        [HttpDelete("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var deleted = await _service.DeleteUserAsync(userId);
            if (!deleted)
                return NotFound("User profile not found.");

            return Ok(new { Message = "Profile deleted successfully." });
        }
    }
}
