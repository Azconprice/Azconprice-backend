using Application.Models.DTOs;
using Application.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementUnitController(IMeasurementUnitService measurementUnitService, IAppLogger appLogger) : ControllerBase
    {
        private readonly IMeasurementUnitService _measurementUnitService = measurementUnitService;
        private readonly IAppLogger _appLogger = appLogger;

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMeasurementUnitsAsync()
        {
            var measurementUnits = await _measurementUnitService.GetAllSync();
            return Ok(measurementUnits);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMeasurementUnitAsync([FromBody] CreateMeasurementUnitRequest request)
        {

            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Unit))
                {
                    return BadRequest("Invalid measurement unit data.");
                }

                await _appLogger.LogAsync(
                    action: "Measurement Unit Added",
                    relatedEntityId: null,
                    userId: $"{User?.FindFirst("userId")}",
                    userName: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")}",
                    details: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")} added Measurement Unit {request.Unit}."
                );

                var result = await _measurementUnitService.AddMeasurementUnitAsync(request);

                return result ? Ok("Measurement unit added successfully.") : BadRequest("Failed to add measurement unit.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }

        }


        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMeasurementUnitAsync([FromQuery] string id, [FromBody] CreateMeasurementUnitRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Unit))
                {
                    return BadRequest("Invalid measurement unit data.");
                }

                await _appLogger.LogAsync(
                   action: "Measurement Unit Updated",
                   relatedEntityId: null,
                   userId: $"{User?.FindFirst("userId")}",
                   userName: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")}",
                   details: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")} updated Measurement Unit {request.Unit}."
                );

                var updatedUnit = await _measurementUnitService.UpdateMeasurementUnitAsync(id, request);
                return updatedUnit != null ? Ok(updatedUnit) : NotFound("Measurement unit not found.");

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMeasurementUnitAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("Invalid measurement unit ID.");
                }
                var result = await _measurementUnitService.DeleteMeasurementUnitAsync(id);

                await _appLogger.LogAsync(
                   action: "Measurement Unit Updated",
                   relatedEntityId: null,
                   userId: $"{User?.FindFirst("userId")}",
                   userName: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")}",
                   details: $"{User?.FindFirst("firstname")} {User?.FindFirst("lastname")} added Measurement Unit with id {id}."
                );

                return result ? Ok("Measurement unit deleted successfully.") : NotFound("Measurement unit not found.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
           
        }
    }
}
