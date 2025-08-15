using System.Text.Json;
using DAL.Models;
using DAL.Repository.Contracts;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Google.Weather;

public class GoogleWeatherDataPersistence(
    IWeatherRepository weatherRepository,
    ILocationRepository locationRepository,
    ILogger logger)
{
    public async Task<WeatherData?> GetValidWeatherAsync(int locationId, CancellationToken cancelToken = default)
    {
        var cachedWeather = await weatherRepository.GetCachedWeatherAsync(locationId, cancelToken);
        
        if (cachedWeather != null)
        {
            logger.LogInformation("Found valid cached weather data for location: {LocationId}", locationId);
        }
        
        return cachedWeather;
    }

    public async Task<Location?> GetLocationAsync(int locationId, CancellationToken cancelToken = default)
    {
        var location = await locationRepository.FindLocationByIdAsync(locationId);
        
        if (location == null)
        {
            logger.LogWarning("Location not found for ID: {LocationId}", locationId);
        }
        
        return location;
    }

    public async Task<WeatherData> SaveWeatherDataAsync(
        int locationId, 
        string rawApiResponse, 
        CancellationToken cancelToken = default)
    {
        var weatherData = MapToWeatherData(locationId, rawApiResponse);
        
        var savedWeatherData = await weatherRepository.SaveWeatherDataAsync(weatherData, cancelToken);
        
        logger.LogInformation("Saved weather data for location: {LocationId}, expires at: {ExpiresAt}", 
            locationId, savedWeatherData.ExpiresAt);
        
        return savedWeatherData;
    }

    public async Task<bool> HasValidWeatherAsync(int locationId, CancellationToken cancelToken = default)
    {
        var cachedWeather = await weatherRepository.GetCachedWeatherAsync(locationId, cancelToken);
        return cachedWeather != null;
    }

    private static WeatherData MapToWeatherData(int locationId, string rawApiResponse)
    {
        var weatherData = new WeatherData
        {
            LocationId = locationId,
            ApiResponse = rawApiResponse,
            FetchedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5) // 5-minute cache TTL
        };

        // Parse JSON directly to extract key fields
        try
        {
            using var doc = JsonDocument.Parse(rawApiResponse);
            var root = doc.RootElement;

            // Extract temperature
            if (root.TryGetProperty("temperature", out var tempElement))
            {
                if (tempElement.TryGetProperty("degrees", out var degreesElement))
                    weatherData.Temperature = degreesElement.GetDouble();
                if (tempElement.TryGetProperty("unit", out var unitElement))
                    weatherData.TemperatureUnit = unitElement.GetString();
            }

            // Extract feels like temperature
            if (root.TryGetProperty("feelsLikeTemperature", out var feelsLikeElement))
            {
                if (feelsLikeElement.TryGetProperty("degrees", out var feelsLikeDegreesElement))
                    weatherData.FeelsLikeTemperature = feelsLikeDegreesElement.GetDouble();
            }

            // Extract humidity
            if (root.TryGetProperty("relativeHumidity", out var humidityElement))
                weatherData.Humidity = humidityElement.GetDouble();

            // Extract weather condition
            if (root.TryGetProperty("weatherCondition", out var conditionElement))
            {
                if (conditionElement.TryGetProperty("description", out var descElement) &&
                    descElement.TryGetProperty("text", out var textElement))
                    weatherData.Condition = textElement.GetString();
                
                if (conditionElement.TryGetProperty("type", out var typeElement))
                    weatherData.ConditionType = typeElement.GetString();
                
                if (conditionElement.TryGetProperty("iconBaseUri", out var iconElement))
                    weatherData.IconUri = iconElement.GetString();
            }

            // Extract UV index
            if (root.TryGetProperty("uvIndex", out var uvElement))
                weatherData.UvIndex = uvElement.GetDouble();

            // Extract wind data
            if (root.TryGetProperty("wind", out var windElement))
            {
                if (windElement.TryGetProperty("speed", out var speedElement))
                {
                    if (speedElement.TryGetProperty("value", out var speedValueElement))
                        weatherData.WindSpeed = speedValueElement.GetDouble();
                    if (speedElement.TryGetProperty("unit", out var speedUnitElement))
                        weatherData.WindSpeedUnit = speedUnitElement.GetString();
                }
                
                if (windElement.TryGetProperty("direction", out var directionElement) &&
                    directionElement.TryGetProperty("degrees", out var directionDegreesElement))
                    weatherData.WindDirection = directionDegreesElement.GetDouble();
            }

            // Extract daytime flag
            if (root.TryGetProperty("isDaytime", out var daytimeElement))
                weatherData.IsDaytime = daytimeElement.GetBoolean();

            // Generate summary
            weatherData.Summary = GenerateWeatherSummary(weatherData);
        }
        catch (JsonException)
        {
            // Log parsing error but don't fail the save
            // We still have the raw response stored
            weatherData.Summary = "Weather data available";
        }

        return weatherData;
    }

    private static string GenerateWeatherSummary(WeatherData weatherData)
    {
        var condition = weatherData.Condition ?? "Unknown";
        var temp = weatherData.Temperature;
        var unit = weatherData.TemperatureUnit ?? "°C";
        
        return temp.HasValue 
            ? $"{condition}, {temp:F1}{unit}" 
            : condition;
    }
}