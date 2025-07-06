using Amazon.Runtime.Internal;
using Application.Models.DTOs;
using Application.Models.DTOs.Excel;
using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.User;
using Application.Models.DTOs.Worker;
using Application.Services;
using FluentValidation;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController(
        IWorkerService service,
        IValidator<WorkerUpdateProfileDTO> updateValidator, IValidator<ChangePasswordDTO> changePasswordValidator,
        IAppLogger appLogger,
        IExcelFileService excelFileService
    ) : ControllerBase
    {
        private readonly IWorkerService _service = service;
        private readonly IValidator<WorkerUpdateProfileDTO> _updateValidator = updateValidator;
        private readonly IValidator<ChangePasswordDTO> _changePasswordValidator = changePasswordValidator;
        private readonly IAppLogger _appLogger = appLogger;
        private readonly IExcelFileService _excelFileService = excelFileService;

        [HttpGet("profile/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<WorkerProfileDTO>> GetProfile(string id)
        {
            var result = await _service.GetWorkerProfile(id);
            if (result is not null)
                return Ok(result);

            return NotFound("Worker profile not found.");
        }

        [HttpGet("list")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<WorkerProfileDTO>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllWorkersAsync(new PaginationRequest { Page = page, PageSize = pageSize });
            return Ok(result);
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

        [HttpPut("profile/change-password")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO updateDto)
        {
            var validationResult = await _changePasswordValidator.ValidateAsync(updateDto);
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
                var result = await _service.ChangeWorkerPasswordAsync(userId, updateDto);
                await _appLogger.LogAsync(
                 action: "Worker Password Change",
                  relatedEntityId: User.FindFirst("userId")?.Value,
                  userId: User.FindFirst("userId")?.Value,
                  userName: $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}",
                  details: $"Worker {User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value} changed password"
                );
                return Ok(new { Message = "Password changed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> PatchMyProfile([FromForm] WorkerUpdateProfileDTO updateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDto);

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
                if (updated is null)
                    return NotFound("Worker profile not found.");

                await _appLogger.LogAsync(
                   action: "Worker Profile Update",
                   relatedEntityId: updated.Id.ToString(),
                   userId: updated.UserId,
                   userName: $"{updated.FirstName} {updated.LastName}",
                   details: $"Worker {updated.FirstName} {updated.LastName} updated profile with ID: {updated.Id}"
               );

                return Ok(updated);
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
            var validationResult = await _updateValidator.ValidateAsync(updateDto);

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
                if (updated is null)
                    return NotFound("Worker profile not found.");

                await _appLogger.LogAsync(
                   action: "Worker Profile Update",
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


        [HttpGet("profile/me/excel")]
        [Authorize(Roles = "Worker")]
        public async Task<ActionResult<PaginatedResult<ExcelFileDTO>>> GetMyFiles([FromQuery] PaginationRequest request)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");
            var result = await _excelFileService.GetExcelFilesByUserAsync(userId, request);
            return Ok(result);
        }

        [HttpDelete("profile/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> DeleteProfile(string id)
        {
            var deleted = await _service.DeleteWorkerProfile(id);
            if (!deleted)
                return NotFound("Worker profile not found.");

            await _appLogger.LogAsync(
                   action: "Worker Profile Delete",
                   relatedEntityId: User.FindFirst("userId")?.Value,
                   userId: User.FindFirst("userId")?.Value,
                   userName: $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}",
                   details: $"Admin {User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value} deleted profile with ID: {User.FindFirst("userId")?.Value}"
            );

            return Ok(new { Message = "Profile deleted successfully." });
        }

        [HttpDelete("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var deleted = await _service.DeleteWorkerProfile(userId);
            if (!deleted)
                return NotFound("Worker profile not found.");

            await _appLogger.LogAsync(
                   action: "Worker Profile Delete",
                   relatedEntityId: User.FindFirst("userId")?.Value,
                   userId: User.FindFirst("userId")?.Value,
                   userName: $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}",
                   details: $"Worker {User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value} deleted profile with ID: {User.FindFirst("userId")?.Value}"
            );

            return Ok(new { Message = "Profile deleted successfully." });
        }
    }
}
