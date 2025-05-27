using Application.Models.DTOs;
using Application.Models.DTOs.Worker;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController(IWorkerService service) : ControllerBase
    {
        private readonly IWorkerService _service = service;

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<ActionResult<WorkerProfileDTO>> GetProfile(string email) {
            var result = await _service.GetWorkerProfile(email);
            if(result is not null)
                return Ok(result);

            return NotFound("Worker profile not found.");
        }
    }
}
