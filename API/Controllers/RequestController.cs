using Microsoft.AspNetCore.Mvc;
using Application.Models.DTOs;
using Application.Models.DTOs.Pagination;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController(
        IRequestService requestService,
        IValidator<RequestDTO> requestValidator
    ) : ControllerBase
    {
        private readonly IRequestService _requestService = requestService;
        private readonly IValidator<RequestDTO> _requestValidator = requestValidator;

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RequestShowDTO>> Create([FromBody] RequestDTO dto)
        {
            var validationResult = await _requestValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }

            var created = await _requestService.CreateRequestAsync(dto);
            if (created is null)
                return StatusCode(500, "Unknown error occured");

            return Ok(created);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _requestService.DeleteRequestAsync(id);
            if (!deleted)
                return NotFound(new { Message = "Request not found." });

            return Ok(new { Message = "Request deleted successfully." });
        }

        [HttpGet("list")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<RequestShowDTO>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _requestService.GetAllRequestsAsync(new PaginationRequest { Page = page, PageSize = pageSize });
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,User,Company,Worker")]
        public async Task<ActionResult<PaginatedResult<RequestShowDTO>>> GetByMail([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var mail = User?.Identity?.Name;
                if (string.IsNullOrEmpty(mail))
                    return Unauthorized();
                var result = await _requestService.GetUserRequestsAsync(new PaginationRequest { Page = page, PageSize = pageSize }, mail);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<RequestShowDTO>> GetById(string id)
        {
            var result = await _requestService.GetRequestByIdAsync(id);
            if (result == null)
                return NotFound(new { Message = "Request not found." });

            return Ok(result);
        }

        [HttpGet("by-type/{type}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<RequestShowDTO>>> GetByType([FromRoute] string type, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _requestService.GetRequestByTypeAsync(new PaginationRequest { Page = page, PageSize = pageSize }, type);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
