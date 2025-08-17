using Application.Models.DTOs.WorkerFunction;
using Application.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerFunctionController(IWorkerFunctionService workerFunctionService, IWorkerFunctionSpecializationService workerFunctionSpecializationService) : ControllerBase
    {
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> GetMyWorkerFunctions()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not found.");
                var result = await workerFunctionService.GetWorkerFunctionsByWorkerProfileIdAsync(userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception (ex) here if needed
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("me")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> AddWorkerFunction([FromBody] CreateWorkerFunctionRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not found.");
                var result = await workerFunctionService.AddWorkerFunctionAsync(userId, request);
                if (result)
                {
                    return Ok("Worker function added successfully.");
                }
                return BadRequest("Failed to add worker function.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }

        }

        [HttpPatch("{id}/add-specialization")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> AddSpecializationToWorkerFunction(string id, [FromBody] string specializationId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not found.");
                var result = await workerFunctionService.AddSpecialization(userId, id, specializationId);
                return result is not null ? Ok("Specialization added successfully.") : NotFound("Worker function not found or specialization already exists.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> UpdateWorkerFunction(string id, [FromBody] CreateWorkerFunctionRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not found.");
                var result = await workerFunctionService.UpdateWorkerFunctionAsync(userId, id, request);
                return result != null ? Ok(result) : NotFound("Worker function not found.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }   

        [HttpDelete("delete/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Worker")]
        public async Task<IActionResult> DeleteWorkerFunction(string id)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not found.");
                var result = await workerFunctionService.DeleteWorkerFunctionAsync(userId, id);
                if (result)
                {
                    return Created();
                }
                return NotFound("Worker function not found.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}