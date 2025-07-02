using Application.Models.DTOs;
using Application.Models.DTOs.Company;
using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.Worker;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController(
        ICompanyService companyService,
        IValidator<UpdateCompanyProfileDTO> updateValidator, IValidator<ChangePasswordDTO> changePasswordValidator,
        IAppLogger appLogger) : ControllerBase
    {
        private readonly ICompanyService _companyService = companyService;
        private readonly IValidator<UpdateCompanyProfileDTO> _updateValidator = updateValidator;
        private readonly IValidator<ChangePasswordDTO> _changePasswordValidator = changePasswordValidator;
        private readonly IAppLogger _appLogger = appLogger;

        [HttpGet("profile/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<CompanyProfileDTO>> GetProfile(string id)
        {
            var result = await _companyService.GetCompanyProfile(id);
            if (result is not null)
                return Ok(result);

            return NotFound("Company profile not found.");
        }

        [HttpGet("list")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<CompanyProfileDTO>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _companyService.GetAllCompaniesAsync(new PaginationRequest { Page = page, PageSize = pageSize });
            return Ok(result);
        }

        [HttpGet("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Company")]
        public async Task<ActionResult<WorkerProfileDTO>> GetMyProfile()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _companyService.GetCompanyProfile(userId);
            if (result is not null)
                return Ok(result);

            return NotFound("Company profile not found.");
        }

        [HttpPut("profile/change-password")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Company")]
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
                var result = await _companyService.ChangeCompanyPasswordAsync(userId, updateDto);
                await _appLogger.LogAsync(
                 action: "Company Password Change",
                  relatedEntityId: User.FindFirst("userId")?.Value,
                  userId: User.FindFirst("userId")?.Value,
                  userName: $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}",
                  details: $"Company {User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value} changed password"
                );
                return Ok(new { Message = "Password changed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPatch("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Company")]
        public async Task<ActionResult<CompanyProfileDTO>> PatchMyProfile([FromForm] UpdateCompanyProfileDTO updateDto)
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
                var updated = await _companyService.UpdateCompanyProfile(
                    userId,
                    updateDto,
                    (userId, token) => Url.Action("ConfirmEmail", "Auth", new { userId, token }, Request.Scheme) ?? string.Empty
                );
                if (updated is null)
                    return NotFound("Company profile not found.");

                await _appLogger.LogAsync(
                   action: "Company Profile Update",
                   relatedEntityId: updated.Id.ToString(),
                   userId: updated.UserId,
                   userName: $"{updated.CompanyName}",
                   details: $"Company {updated.CompanyName} updated profile with ID: {updated.Id}"
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
        public async Task<IActionResult> PatchProfile(string id, [FromForm] UpdateCompanyProfileDTO updateDto)
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
                var updated = await _companyService.UpdateCompanyProfile(
                    id,
                    updateDto,
                    (userId, token) => Url.Action("ConfirmEmail", "Auth", new { userId, token }, Request.Scheme) ?? string.Empty
                );
                if (updated is null)
                    return NotFound("Company profile not found.");

                await _appLogger.LogAsync(
                   action: "Company Profile Update",
                   relatedEntityId: updated.Id.ToString(),
                   userId: updated.UserId,
                   userName: $"{updated.CompanyName}",
                   details: $"Admin updated company profile with ID: {updated.Id}"
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
            var deleted = await _companyService.DeleteCompanyProfile(id);
            if (!deleted)
                return NotFound("Company profile not found.");

            await _appLogger.LogAsync(
                   action: "Company Profile Delete",
                   relatedEntityId: id,
                   userId: User.FindFirst("userId")?.Value,
                   userName: $"{User.FindFirst("firstname")?.Value}",
                   details: $"Admin deleted company profile with ID: {id}"
            );

            return Ok(new { Message = "Profile deleted successfully." });
        }

        [HttpDelete("profile/me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Company")]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var deleted = await _companyService.DeleteCompanyProfile(userId);
            if (!deleted)
                return NotFound("Company profile not found.");

            await _appLogger.LogAsync(
                   action: "Company Profile Delete",
                   relatedEntityId: userId,
                   userId: userId,
                   userName: $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}",
                   details: $"Company deleted own profile with ID: {userId}"
            );

            return Ok(new { Message = "Profile deleted successfully." });
        }
    }
}
