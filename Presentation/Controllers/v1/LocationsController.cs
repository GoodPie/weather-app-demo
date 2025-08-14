using BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly ILogger<WeatherForecastController> _logger;

    public LocationsController(ILogger<WeatherForecastController> logger, ILocationService locationService)
    {
        _logger = logger;
        _locationService = locationService;
    }

    [HttpGet(Name = "SearchByCity")]
    [Route("search")]
    public async Task<IActionResult> SearchByCity([FromQuery] string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName)) return BadRequest("City name cannot be null or empty.");

        var response = await _locationService.FindLocationByCityAsync(cityName);
        if (!response.Success) return NotFound(response.Message);

        return Ok(response.Data);
    }
}