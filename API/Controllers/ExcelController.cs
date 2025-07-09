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
    public class ExcelController(IExcelFileService excelFileService, UserManager<User> userManager, IAppLogger appLogger, IExcelMatchService svc) : ControllerBase
    {
        private readonly IExcelFileService _excelFileService = excelFileService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IAppLogger _appLogger = appLogger;
        private readonly IExcelMatchService _svc = svc;

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

        [HttpPost("upload")]
        [AllowAnonymous]
        public IActionResult UploadExcel(ExcelUploadRequest request)
        {
            // 1. Extract claims
            var userId = User.FindFirst("userId")?.Value;
            var email = User.Identity?.Name;
            var firstName = User.FindFirst("firstName")?.Value;
            var lastName = User.FindFirst("lastName")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // 2. Validate file type
            if (!request.File.FileName.EndsWith(".xlsx") && !request.File.FileName.EndsWith(".xls"))
                return BadRequest("Only Excel files are supported.");

            if (role is not null && role is "Company")
            {
                lastName = null;
            }

            if (request.File == null || request.File.Length == 0)
                return BadRequest("File is required.");

            var result = _excelFileService.ProcessQueryExcelAsync(request.File, userId, request.IsSimple);

            return result; // ✅ Return directly

            //await _appLogger.LogAsync(
            //       action: "Uploaded Excel File",
            //       relatedEntityId: User.FindFirst("userId")?.Value,
            //       userId: User.FindFirst("userId")?.Value,
            //       userName: role == "Company" ? $"{User.FindFirst("firstname")?.Value}" : $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}",
            //       details: role == "Company" ? $"{User.FindFirst("firstname")?.Value} uploaded excel file" : $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value} uploaded excel file"
            //);
        }

        //[HttpPost("upload")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Upload([FromForm] ExcelUploadRequest request)
        //{
        //    var (bytes, fileName, ctype) = await _svc.ProcessQueryExcelAsync(request.File);
        //    return File(bytes, ctype, fileName);
        //}
    }
}
