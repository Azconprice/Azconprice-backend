using Application.Models.DTOs.SalesCategory;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesCategoryController(ISalesCategoryService salesCategoryService) : ControllerBase
    {
        private readonly ISalesCategoryService _salesCategoryService = salesCategoryService;

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SalesCategoryShowDTO>>> GetAll()
        {
            var result = await _salesCategoryService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalesCategoryShowDTO>> GetById(string id)
        {
            var result = await _salesCategoryService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<SalesCategoryShowDTO>> Create([FromBody] CreateSalesCategoryDTO dto)
        {
            try
            {
                var created = await _salesCategoryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<SalesCategoryShowDTO>> Update(string id, [FromBody] UpdateSalesCategoryDTO dto)
        {
            try
            {
                var updated = await _salesCategoryService.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _salesCategoryService.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
