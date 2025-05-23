using Application.Models;
using Application.Models.DTOs;
using Application.Models.DTOs.Category;
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

        [HttpPost("addCategory")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<bool>> AddCategory([FromBody] ProfessionDTO model) => await _service.AddProfessionAsync(model);


        [HttpGet("showAllCategories")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public ActionResult<IEnumerable<ProfessionShowDTO>> GetAllProfessions() => Ok(_service.GetAllProfessions());

        [HttpPut("updateCategory/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateCategory([FromBody] ProfessionUpdateDTO model, [FromRoute] string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);
            return await _service.UpdateProfessionAsync(model, id);
        }

        [HttpPost("addAdmin")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<bool>> AddAdmin([FromBody] AddAdminDTO model) => await _service.AddNewAdmin(model);
    }
}
