using DAL.Models;

namespace BLL.Tests.Builders;

public class WeatherDataBuilder
{
    private readonly WeatherData _weatherData;

    private WeatherDataBuilder()
    {
        _weatherData = new WeatherData
        {
            Id = 1,
            LocationId = 1,
            ApiResponse = "{}",
            FetchedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            Temperature = 20.0,
            TemperatureUnit = "CELSIUS",
            FeelsLikeTemperature = 22.0,
            Humidity = 65.0,
            Condition = "Partly cloudy",
            ConditionType = "PARTLY_CLOUDY",
            IconUri = "https://example.com/icon.png",
            UvIndex = 5.0,
            WindSpeed = 10.0,
            WindSpeedUnit = "METERS_PER_SECOND",
            WindDirection = 180.0,
            IsDaytime = true,
            Summary = "Partly cloudy, 20.0°C",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static WeatherDataBuilder Create() => new();

    public WeatherDataBuilder WithLocationId(int locationId)
    {
        _weatherData.LocationId = locationId;
        return this;
    }

    public WeatherDataBuilder WithTemperature(double temperature, string unit = "CELSIUS")
    {
        _weatherData.Temperature = temperature;
        _weatherData.TemperatureUnit = unit;
        return this;
    }

    public WeatherDataBuilder WithValidCache()
    {
        _weatherData.ExpiresAt = DateTime.UtcNow.AddMinutes(5); // Valid for 5 more minutes
        return this;
    }

    public WeatherDataBuilder WithExpiredCache()
    {
        _weatherData.ExpiresAt = DateTime.UtcNow.AddMinutes(-5); // Expired 5 minutes ago
        return this;
    }

    public WeatherDataBuilder WithCondition(string conditionText, string conditionType)
    {
        _weatherData.Condition = conditionText;
        _weatherData.ConditionType = conditionType;
        return this;
    }

    public WeatherDataBuilder WithWind(double speed, string unit, double direction)
    {
        _weatherData.WindSpeed = speed;
        _weatherData.WindSpeedUnit = unit;
        _weatherData.WindDirection = direction;
        return this;
    }

    public WeatherDataBuilder WithLocation(Location location)
    {
        _weatherData.Location = location;
        _weatherData.LocationId = location.Id;
        return this;
    }

    public WeatherData Build() => _weatherData;
}