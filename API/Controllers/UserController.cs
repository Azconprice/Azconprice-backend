using Application.Models.DTOs.Pagination;
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
    public class UserController(IClientService service, IValidator<UserUpdateDTO> validator, IAppLogger appLogger) : Controller
    {
        private readonly IClientService _service = service;
        private readonly IValidator<UserUpdateDTO> _validator = validator;
        private readonly IAppLogger _appLogger = appLogger;

        [HttpGet("list")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<UserShowDTO>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllUsersAsync(new PaginationRequest { Page = page, PageSize = pageSize });
            return Ok(result);
        }

        [HttpGet("profile/{id}")]
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
        public async Task<ActionResult<UserShowDTO>> GetMyProfile()
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
                var updated = await _service.UpdateUserAsync(userId, updateDto, (email, token) => Url.Action("ConfirmEmail", "Auth", new { email, token }, Request.Scheme) ?? string.Empty);
                if (updated is null)
                    return NotFound("User profile not found.");

                await _appLogger.LogAsync(
                  action: "User Profile Update",
                  relatedEntityId: updated.Id.ToString(),
                  userId: updated.Id,
                  userName: $"{updated.FirstName} {updated.LastName}",
                  details: $"Worker {updated.FirstName} {updated.LastName} updated profile with ID: {updated.Id}"
                );

                return Ok(new { Message = "Profile updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch("profile/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
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
                var updated = await _service.UpdateUserAsync(id, updateDto, (email, token) => Url.Action("ConfirmEmail", "Auth", new { email, token }, Request.Scheme) ?? string.Empty);
                if (updated is null)
                    return NotFound("User profile not found.");

                await _appLogger.LogAsync(
                   action: "User Profile Update",
                   relatedEntityId: User.FindFirst("userId")?.Value,
                   userId: User.FindFirst("userId")?.Value,
                   userName: $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}",
                   details: $"Admin {User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value} updated profile with ID: {updated.Id}"
               );


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
            var deleted = await _service.DeleteUserAsync(id);
            if (!deleted)
                return NotFound("User profile not found.");

            await _appLogger.LogAsync(
                   action: "User Profile Delete",
                   relatedEntityId: User.FindFirst("userId")?.Value,
                   userId: User.FindFirst("userId")?.Value,
                   userName: $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}",
                   details: $"Admin {User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value} deleted profile with ID: {User.FindFirst("userId")?.Value}"
            );

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

            await _appLogger.LogAsync(
                  action: "User Profile Delete",
                  relatedEntityId: User.FindFirst("userId")?.Value,
                  userId: User.FindFirst("userId")?.Value,
                  userName: $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}",
                  details: $"Worker {User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value} deleted profile with ID: {User.FindFirst("userId")?.Value}"
           );

            return Ok(new { Message = "Profile deleted successfully." });
        }
    }
}
