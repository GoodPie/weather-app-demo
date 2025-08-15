using BLL.Services.Contracts;
using DAL.Dtos.Weather;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class WeatherController(
    ILogger<WeatherController> logger,
    IWeatherService weatherService,
    ILocationService locationService)
    : ControllerBase
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentWeather(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] string units = "metric")
    {
        logger.LogInformation("Getting current weather for coordinates: {Latitude}, {Longitude}", latitude, longitude);

        try
        {
            // Find location by coordinates
            var locationResponse = await locationService.FindLocationByGeolocationAsync(latitude, longitude);
            if (!locationResponse.Success || locationResponse.Data == null || !locationResponse.Data.Any())
            {
                logger.LogWarning("No location found for coordinates: {Latitude}, {Longitude}", latitude, longitude);
                return NotFound("Location not found for the specified coordinates");
            }

            var location = locationResponse.Data.First();
            var weatherResponse = await weatherService.GetCurrentWeatherByLocationIdAsync(location.Id);

            if (!weatherResponse.Success)
            {
                logger.LogWarning("Failed to get weather data: {Message}", weatherResponse.Message);
                return NotFound(weatherResponse.Message);
            }


            var response = new WeatherResponseDto<CurrentWeatherDto>
            {
                Weather = weatherResponse.Data,
                Location = location
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting current weather for coordinates: {Latitude}, {Longitude}", latitude,
                longitude);
            return StatusCode(500, "An error occurred while retrieving weather data");
        }
    }

    [HttpGet("current/{locationId:int}")]
    public async Task<IActionResult> GetCurrentWeatherByLocationId(int locationId)
    {
        logger.LogInformation("Getting current weather for location ID: {LocationId}", locationId);

        try
        {
            var location = await locationService.FindLocationByIdAsync(locationId);
            var weatherResponse = await weatherService.GetCurrentWeatherByLocationIdAsync(locationId);

            if (!weatherResponse.Success)
            {
                logger.LogWarning("Failed to get weather data for location {LocationId}: {Message}", locationId,
                    weatherResponse.Message);
                return NotFound(weatherResponse.Message);
            }

            var response = new WeatherResponseDto<CurrentWeatherDto>
            {
                Weather = weatherResponse.Data,
                Location = location.Data
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting current weather for location ID: {LocationId}", locationId);
            return StatusCode(500, "An error occurred while retrieving weather data");
        }
    }

    [HttpGet("forecast/hourly")]
    public async Task<IActionResult> GetHourlyForecast(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] int hours = 24,
        [FromQuery] string units = "metric")
    {
        logger.LogInformation("Getting hourly forecast for coordinates: {Latitude}, {Longitude}, hours: {Hours}",
            latitude, longitude, hours);

        try
        {
            // Find location by coordinates
            var locationResponse = await locationService.FindLocationByGeolocationAsync(latitude, longitude);
            if (!locationResponse.Success || locationResponse.Data == null || !locationResponse.Data.Any())
            {
                logger.LogWarning("No location found for coordinates: {Latitude}, {Longitude}", latitude, longitude);
                return NotFound("Location not found for the specified coordinates");
            }

            var location = locationResponse.Data.First();
            var forecastResponse = await weatherService.GetHourlyForecastByLocationIdAsync(location.Id, hours);

            if (!forecastResponse.Success)
            {
                logger.LogWarning("Failed to get hourly forecast: {Message}", forecastResponse.Message);
                return NotFound(forecastResponse.Message);
            }

            var response = new WeatherResponseDto<List<HourlyForecastDto>>
            {
                Weather = forecastResponse.Data,
                Location = location
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting hourly forecast for coordinates: {Latitude}, {Longitude}", latitude,
                longitude);
            return StatusCode(500, "An error occurred while retrieving hourly forecast");
        }
    }

    [HttpGet("forecast/daily")]
    public async Task<IActionResult> GetDailyForecast(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] int days = 7,
        [FromQuery] string units = "metric")
    {
        logger.LogInformation("Getting daily forecast for coordinates: {Latitude}, {Longitude}, days: {Days}", latitude,
            longitude, days);

        try
        {
            // Find location by coordinates
            var locationResponse = await locationService.FindLocationByGeolocationAsync(latitude, longitude);
            if (!locationResponse.Success || locationResponse.Data == null || !locationResponse.Data.Any())
            {
                logger.LogWarning("No location found for coordinates: {Latitude}, {Longitude}", latitude, longitude);
                return NotFound("Location not found for the specified coordinates");
            }

            var location = locationResponse.Data.First();
            var forecastResponse = await weatherService.GetDailyForecastByLocationIdAsync(location.Id, days);

            if (!forecastResponse.Success)
            {
                logger.LogWarning("Failed to get daily forecast: {Message}", forecastResponse.Message);
                return NotFound(forecastResponse.Message);
            }

            var response = new WeatherResponseDto<List<DailyForecastDto>>
            {
                Weather = forecastResponse.Data,
                Location = location
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting daily forecast for coordinates: {Latitude}, {Longitude}", latitude,
                longitude);
            return StatusCode(500, "An error occurred while retrieving daily forecast");
        }
    }

    [HttpGet("bundle")]
    public async Task<IActionResult> GetWeatherBundle(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] string units = "metric")
    {
        logger.LogInformation("Getting weather bundle for coordinates: {Latitude}, {Longitude}", latitude, longitude);

        try
        {
            // Find location by coordinates
            var locationResponse = await locationService.FindLocationByGeolocationAsync(latitude, longitude);
            if (!locationResponse.Success || locationResponse.Data == null || !locationResponse.Data.Any())
            {
                logger.LogWarning("No location found for coordinates: {Latitude}, {Longitude}", latitude, longitude);
                return NotFound("Location not found for the specified coordinates");
            }

            var location = locationResponse.Data.First();
            var bundleResponse = await weatherService.GetWeatherBundleByLocationIdAsync(location.Id);

            if (!bundleResponse.Success)
            {
                logger.LogWarning("Failed to get weather bundle: {Message}", bundleResponse.Message);
                return NotFound(bundleResponse.Message);
            }

            var response = new WeatherResponseDto<WeatherBundleDto>
            {
                Weather = bundleResponse.Data,
                Location = location
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting weather bundle for coordinates: {Latitude}, {Longitude}", latitude,
                longitude);
            return StatusCode(500, "An error occurred while retrieving weather bundle");
        }
    }

    [HttpGet("bundle/{locationId:int}")]
    public async Task<IActionResult> GetWeatherBundleByLocationId(int locationId)
    {
        logger.LogInformation("Getting weather bundle for location ID: {LocationId}", locationId);

        try
        {
            var locationResponse = await locationService.FindLocationByIdAsync(locationId);
            var bundleResponse = await weatherService.GetWeatherBundleByLocationIdAsync(locationId);

            if (!bundleResponse.Success)
            {
                logger.LogWarning("Failed to get weather bundle for location {LocationId}: {Message}", locationId,
                    bundleResponse.Message);
                return NotFound(bundleResponse.Message);
            }


            var response = new WeatherResponseDto<WeatherBundleDto>
            {
                Weather = bundleResponse.Data,
                Location = locationResponse.Data
            };


            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting weather bundle for location ID: {LocationId}", locationId);
            return StatusCode(500, "An error occurred while retrieving weather bundle");
        }
    }
}