using Application.Models.DTOs.Excel;
using Application.Models.DTOs.Pagination;
using Application.Services;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExcelController(IExcelFileService excelFileService, UserManager<User> userManager, IAppLogger appLogger) : ControllerBase
    {
        private readonly IExcelFileService _excelFileService = excelFileService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IAppLogger _appLogger = appLogger;

        [HttpGet("list")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<ExcelFileDTO>>> Get([FromQuery] PaginationRequest request)
        {
            var result = await _excelFileService.GetExcelFilesAsync(request);
            return Ok(result);
        }

        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> GetCount()
        {
            var count = await _excelFileService.GetExcelFileCountAsync();
            return Ok(count);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Admin,Company,User,Worker")]
        public async Task<ActionResult<PaginatedResult<ExcelFileDTO>>> GetMyFiles([FromQuery] PaginationRequest request)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");
            var result = await _excelFileService.GetExcelFilesByUserAsync(userId, request);
            return Ok(result);
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin,Company,User,Worker")]
        public async Task<ActionResult<ExcelFileDTO>> UploadExcel(ExcelUploadRequest request)
        {
            // 1. Extract claims
            var userId = User.FindFirst("userId")?.Value;
            var email = User.Identity?.Name;
            var firstName = User.FindFirst("firstName")?.Value;
            var lastName = User.FindFirst("lastName")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(email) || request.File is null)
                return Unauthorized("Missing identity information or file.");

            // 2. Validate file type
            if (!request.File.FileName.EndsWith(".xlsx") && !request.File.FileName.EndsWith(".xls"))
                return BadRequest("Only Excel files are supported.");

            // 3. Adjust names based on role
            if (role == "Company")
            {
                lastName = null;
            }

            await _appLogger.LogAsync(
                   action: "Uploaded Excel File",
                   relatedEntityId: User.FindFirst("userId")?.Value,
                   userId: User.FindFirst("userId")?.Value,
                   userName: role == "Company" ? $"{User.FindFirst("firstname")?.Value}" : $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}",
                   details: role == "Company" ? $"{User.FindFirst("firstname")?.Value} uploaded excel file" : $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value} uploaded excel file"
            );

            var result = await _excelFileService.UploadExcelAsync(request.File, firstName!, lastName ?? "", email!, userId);
            return Ok(result);
        }
    }
}
