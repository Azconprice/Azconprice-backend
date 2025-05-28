using Application.Models.DTOs;
using Application.Models.DTOs.AppLogs;
using Application.Models.DTOs.Pagination;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public class AdminController(IAdminService service, IAppLogger appLogger) : ControllerBase
    {
        private readonly IAdminService _service = service;
        private readonly IAppLogger _appLogger = appLogger;

        [HttpPost("add-admin")]
        public async Task<ActionResult<bool>> AddAdmin([FromBody] AddAdminDTO model)
        {
            try
            {
                var result = await _service.AddNewAdmin(model);
                if (result)
                {
                    await _appLogger.LogAsync(
                        action: "Admin Added",
                        relatedEntityId: null,
                        userId: User?.Identity?.Name,
                        userName: User?.Identity?.Name,
                        details: $"Admin with email '{model.Email}' was added by {User?.Identity?.Name}"
                    );
                    return Created();
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return BadRequest("Failed to add new admin.");
        }

        [HttpPatch("change-company-status/{id}")]
        public async Task<ActionResult<bool>> ChangeCompanyStatus(string id)
        {
            try
            {
                var result = await _service.ChangeCompanyStatus(id);
                if (result)
                {
                    await _appLogger.LogAsync(
                        action: "Company Status Changed",
                        relatedEntityId: id,
                        userId: User?.Identity?.Name,
                        userName: User?.Identity?.Name,
                        details: $"Company status changed for company ID '{id}' by {User?.Identity?.Name}"
                    );
                    return Ok(new { Message = "Company status updated successfully." });
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return NotFound("Company not found");
        }

        [HttpGet("logs")]
        public async Task<ActionResult<IEnumerable<PaginatedResult<LogListItemDTO>>>> GetLogs([FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                var request = new PaginationRequest
                {
                    Page = page,
                    PageSize = pageSize
                };
                var logs = await _service.GetLogsAsync(request);
                if (logs is not null && logs.Items.Any())
                {
                    return Ok(logs);
                }
                return NotFound("No logs found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
