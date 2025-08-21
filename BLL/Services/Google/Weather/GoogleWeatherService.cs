using BLL.Services.Contracts;
using BLL.Services.Weather;
using DAL.Dtos;
using DAL.Dtos.Weather;
using DAL.Models;
using DAL.Repository.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Google.Weather;

public class GoogleWeatherService : IWeatherService
{
    private readonly GoogleApiClient _apiClient;
    private readonly GoogleWeatherDataPersistence _dataPersistence;
    private readonly ILogger<GoogleWeatherService> _logger;
    private readonly string _weatherBaseUrl;

    public GoogleWeatherService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GoogleWeatherService> logger,
        IWeatherRepository weatherRepository,
        ILocationRepository locationRepository)
    {
        _logger = logger;

        var apiKey = configuration["GoogleWeather:ApiKey"]
                     ?? configuration["GoogleMaps:ApiKey"] // Fallback to same key
                     ?? throw new InvalidOperationException("Google Weather API key not found in configuration");

        _weatherBaseUrl = configuration["GoogleWeather:BaseUrl"]
                          ?? throw new InvalidOperationException("Google Weather Base URL not found in configuration");

        // Note: GoogleApiClient expects geocoding baseUrl in constructor, we'll pass weather URL to FetchWeatherDataAsync
        var geocodingBaseUrl = configuration["GoogleMaps:BaseUrl"] ?? "";
        _apiClient = new GoogleApiClient(httpClient, apiKey, geocodingBaseUrl, logger);
        _dataPersistence = new GoogleWeatherDataPersistence(
            weatherRepository,
            locationRepository,
            logger);
    }

    public async Task<ServiceResponse<CurrentWeatherDto>> GetCurrentWeatherByLocationIdAsync(int locationId,
        CancellationToken cancellationToken = default)
    {
        var response = new ServiceResponse<CurrentWeatherDto>();

        try
        {
            // Check if we have valid cached weather data
            var cachedWeather = await _dataPersistence.GetValidWeatherAsync(locationId, cancellationToken);
            if (cachedWeather != null)
            {
                _logger.LogInformation("Returning cached weather data for location: {LocationId}", locationId);
                response.Data = MapToCurrentWeatherDto(cachedWeather);
                response.Success = true;
                response.Message = "Weather data retrieved from cache";
                return response;
            }

            // Get location coordinates for API call
            var location = await _dataPersistence.GetLocationAsync(locationId, cancellationToken);
            if (location == null)
            {
                response.Success = false;
                response.Message = "Location not found";
                return response;
            }

            // Fetch fresh weather data from Google Weather API
            _logger.LogInformation("Fetching fresh weather data for location: {LocationId} ({Latitude}, {Longitude})",
                locationId, location.Latitude, location.Longitude);

            await FetchAndStoreWeatherAsync(locationId, location.Latitude, location.Longitude, cancellationToken);

            // Retrieve the newly saved weather data
            var freshWeather = await _dataPersistence.GetValidWeatherAsync(locationId, cancellationToken);
            if (freshWeather != null)
            {
                response.Data = MapToCurrentWeatherDto(freshWeather);
                response.Success = true;
                response.Message = "Weather data retrieved successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Failed to retrieve weather data after API call";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weather data for location: {LocationId}", locationId);
            response.Success = false;
            response.Message = "An error occurred while retrieving weather data";
        }

        return response;
    }

    public async Task<ServiceResponse<List<HourlyForecastDto>>> GetHourlyForecastByLocationIdAsync(int locationId,
        int hours = 24, CancellationToken cancellationToken = default)
    {
        // For now, return empty list as Google Weather API current conditions endpoint doesn't provide hourly forecast
        // This would require a separate forecast endpoint call
        var response = new ServiceResponse<List<HourlyForecastDto>>
        {
            Data = new List<HourlyForecastDto>(),
            Success = true,
            Message = "Hourly forecast not yet implemented"
        };
        return response;
    }

    public async Task<ServiceResponse<List<DailyForecastDto>>> GetDailyForecastByLocationIdAsync(int locationId,
        int days = 7, CancellationToken cancellationToken = default)
    {
        // For now, return empty list as Google Weather API current conditions endpoint doesn't provide daily forecast
        // This would require a separate forecast endpoint call
        var response = new ServiceResponse<List<DailyForecastDto>>
        {
            Data = new List<DailyForecastDto>(),
            Success = true,
            Message = "Daily forecast not yet implemented"
        };
        return response;
    }

    public async Task<ServiceResponse<WeatherBundleDto>> GetWeatherBundleByLocationIdAsync(int locationId,
        CancellationToken cancellationToken = default)
    {
        var response = new ServiceResponse<WeatherBundleDto>();

        try
        {
            // Get current weather
            var currentWeatherResponse = await GetCurrentWeatherByLocationIdAsync(locationId, cancellationToken);

            response.Data = new WeatherBundleDto
            {
                Current = currentWeatherResponse.Success ? currentWeatherResponse.Data : null,
                Hourly = new List<HourlyForecastDto>(), // TODO: Implement when forecast endpoints added
                Daily = new List<DailyForecastDto>() // TODO: Implement when forecast endpoints added
            };

            response.Success = true;
            response.Message = "Weather bundle retrieved (current only)";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weather bundle for location: {LocationId}", locationId);
            response.Success = false;
            response.Message = "An error occurred while retrieving weather bundle";
        }

        return response;
    }

    private async Task FetchAndStoreWeatherAsync(int locationId, double latitude, double longitude,
        CancellationToken cancellationToken)
    {
        var json = await _apiClient.FetchWeatherDataAsync(latitude, longitude, _weatherBaseUrl, cancellationToken);
        await _dataPersistence.SaveWeatherDataAsync(locationId, json, cancellationToken);
    }

    private static CurrentWeatherDto MapToCurrentWeatherDto(WeatherData weatherData)
    {
        var (temperatureCelsius, temperatureFahrenheit) =
            WeatherUnitConverter.NormalizeTemperature(weatherData.Temperature, weatherData.TemperatureUnit);
        var (feelsLikeCelsius, feelsLikeFahrenheit) =
            WeatherUnitConverter.NormalizeTemperature(weatherData.FeelsLikeTemperature, weatherData.TemperatureUnit);
        var (windKilometersPerHour, windMilesPerHour) =
            WeatherUnitConverter.NormalizeWindSpeed(weatherData.WindSpeed, weatherData.WindSpeedUnit);

        return new CurrentWeatherDto
        {
            AsOf = weatherData.FetchedAt.ToString("O"), // ISO 8601 format
            TempC = temperatureCelsius,
            TempF = temperatureFahrenheit,
            FeelsLikeC = feelsLikeCelsius,
            FeelsLikeF = feelsLikeFahrenheit,
            ConditionCode = weatherData.ConditionType,
            ConditionText = weatherData.Condition,
            WindKph = windKilometersPerHour,
            WindMph = windMilesPerHour,
            WindDir = weatherData.WindDirection.HasValue
                ? WeatherUnitConverter.DegreesToCardinalDirection(weatherData.WindDirection.Value)
                : null,
            Humidity = weatherData.Humidity,
            PressureMb = null, // Not available in Google Weather API current conditions
            UvIndex = weatherData.UvIndex,
            VisibilityKm = null, // Not available in Google Weather API current conditions
            Sunrise = null, // Not available in Google Weather API current conditions
            Sunset = null, // Not available in Google Weather API current conditions
            IsDay = weatherData.IsDaytime
        };
    }
}