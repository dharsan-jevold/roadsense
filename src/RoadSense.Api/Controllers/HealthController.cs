using Microsoft.AspNetCore.Mvc;

namespace RoadSense.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            service = "RoadSense.Api"
        });
    }
}