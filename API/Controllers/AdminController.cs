using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public class AdminController(IAdminService service) : ControllerBase
    {
        private readonly IAdminService _service = service;

        [HttpPost("add-admin")]
        public async Task<ActionResult<bool>> AddAdmin([FromBody] AddAdminDTO model)
        {
            try
            {
                var result = await _service.AddNewAdmin(model);
                if (result)
                    return Created();
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
        public async Task<ActionResult<bool>>  ChangeCompanyStatus(string id)
        {
            try
            {
                var result = await _service.ChangeCompanyStatus(id);
                if (result)
                    return Ok(new { Message = "Company status updated successfully." });
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
    }
}
