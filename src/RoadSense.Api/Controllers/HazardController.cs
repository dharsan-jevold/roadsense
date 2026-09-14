using Microsoft.AspNetCore.Mvc;
using RoadSense.Application.Services;
using RoadSense.Domain.Entities;

namespace RoadSense.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HazardController : ControllerBase
{
    private readonly IHazardService _hazardService;

    public HazardController(IHazardService hazardService)
    {
        _hazardService = hazardService;
    }

    [HttpPost]
    public IActionResult ReportHazard(
        [FromBody] ReportHazardRequest request)
    {
        if (!Enum.IsDefined(request.Type))
        {
            return BadRequest(new
            {
                error = "Invalid hazard type."
            });
        }

        if (request.Latitude < -90 || request.Latitude > 90)
        {
            return BadRequest(new
            {
                error = "Latitude must be between -90 and 90."
            });
        }

        if (request.Longitude < -180 || request.Longitude > 180)
        {
            return BadRequest(new
            {
                error = "Longitude must be between -180 and 180."
            });
        }

        if (request.Severity < 1 || request.Severity > 5)
        {
            return BadRequest(new
            {
                error = "Severity must be between 1 and 5."
            });
        }

        var hazard = _hazardService.CreateHazard(
            request.Type,
            request.Latitude,
            request.Longitude,
            request.Severity);

        return Created(
            $"/api/hazard/{hazard.Id}",
            hazard);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetHazard(Guid id)
    {
        var hazard = _hazardService.GetHazard(id);

        if (hazard is null)
        {
            return NotFound(new
            {
                error = "Hazard not found."
            });
        }

        return Ok(hazard);
    }
}

public record ReportHazardRequest(
    HazardType Type,
    double Latitude,
    double Longitude,
    int Severity);