using Microsoft.AspNetCore.Mvc;
using RoadSense.Api.DTOs;
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
    public async Task<IActionResult> ReportHazard(
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

        var hazard = await _hazardService.CreateHazardAsync(
            request.Type,
            request.Latitude,
            request.Longitude,
            request.Severity);

        var response = ToResponse(hazard);

        return Created(
            $"/api/hazard/{hazard.Id}",
            response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetHazard(Guid id)
    {
        var hazard = await _hazardService.GetHazardAsync(id);

        if (hazard is null)
        {
            return NotFound(new
            {
                error = "Hazard not found."
            });
        }

        return Ok(ToResponse(hazard));
    }

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearbyHazards(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusMeters = 1000)
    {
        if (latitude < -90 || latitude > 90)
        {
            return BadRequest(new
            {
                error = "Latitude must be between -90 and 90."
            });
        }

        if (longitude < -180 || longitude > 180)
        {
            return BadRequest(new
            {
                error = "Longitude must be between -180 and 180."
            });
        }

        if (radiusMeters <= 0)
        {
            return BadRequest(new
            {
                error = "Radius must be greater than 0."
            });
        }

        var hazards = await _hazardService.GetNearbyHazardsAsync(
            latitude,
            longitude,
            radiusMeters);

        var response = hazards
            .Select(ToResponse)
            .ToList();

        return Ok(response);
    }

    private static HazardResponse ToResponse(Hazard hazard)
{
    var now = DateTime.UtcNow;

    return new HazardResponse(
        hazard.Id,
        hazard.Type,
        hazard.Latitude,
        hazard.Longitude,
        hazard.Severity,
        hazard.ReportCount,
        hazard.GetConfidenceScore(now),
        hazard.GetConfidenceLevel(now),
        hazard.ReportedAtUtc,
        hazard.LastReportedAtUtc);
}
}