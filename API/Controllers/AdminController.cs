using Application.Models;
using Application.Models.DTOs;
using Application.Models.DTOs.Profession;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IAdminService service) : ControllerBase
    {
        private readonly IAdminService _service = service;

        [HttpPost("addAdmin")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
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
    }
}
