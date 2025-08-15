using UnitsNet;

namespace BLL.Services.Weather;

public static class WeatherUnitConverter
{
    private static readonly string[] CardinalDirections = 
    { 
        "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", 
        "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" 
    };

    public static double CelsiusToFahrenheit(double temperatureCelsius)
    {
        var temperature = Temperature.FromDegreesCelsius(temperatureCelsius);
        return temperature.DegreesFahrenheit;
    }

    public static double FahrenheitToCelsius(double temperatureFahrenheit)
    {
        var temperature = Temperature.FromDegreesFahrenheit(temperatureFahrenheit);
        return temperature.DegreesCelsius;
    }

    public static double KilometersPerHourToMilesPerHour(double kilometersPerHour)
    {
        var speed = Speed.FromKilometersPerHour(kilometersPerHour);
        return speed.MilesPerHour;
    }

    public static double MilesPerHourToKilometersPerHour(double milesPerHour)
    {
        var speed = Speed.FromMilesPerHour(milesPerHour);
        return speed.KilometersPerHour;
    }

    public static double MetersPerSecondToKilometersPerHour(double metersPerSecond)
    {
        var speed = Speed.FromMetersPerSecond(metersPerSecond);
        return speed.KilometersPerHour;
    }

    public static double MetersPerSecondToMilesPerHour(double metersPerSecond)
    {
        var speed = Speed.FromMetersPerSecond(metersPerSecond);
        return speed.MilesPerHour;
    }

    public static string DegreesToCardinalDirection(double degrees)
    {
        var angle = Angle.FromDegrees(degrees);
        var normalizedDegrees = angle.Degrees % 360;
        if (normalizedDegrees < 0) normalizedDegrees += 360;
        
        var index = (int)Math.Round(normalizedDegrees / 22.5) % 16;
        return CardinalDirections[index];
    }

    public static (double temperatureCelsius, double temperatureFahrenheit) NormalizeTemperature(double? temperature, string? unit)
    {
        if (!temperature.HasValue) return (0, 32);

        return unit?.ToUpperInvariant() switch
        {
            "FAHRENHEIT" => (FahrenheitToCelsius(temperature.Value), temperature.Value),
            "CELSIUS" => (temperature.Value, CelsiusToFahrenheit(temperature.Value)),
            _ => (temperature.Value, CelsiusToFahrenheit(temperature.Value)) // Default assume Celsius
        };
    }

    public static (double? kilometersPerHour, double? milesPerHour) NormalizeWindSpeed(double? windSpeed, string? unit)
    {
        if (!windSpeed.HasValue) return (null, null);

        return unit?.ToUpperInvariant() switch
        {
            "KPH" or "KILOMETERS_PER_HOUR" => (windSpeed.Value, KilometersPerHourToMilesPerHour(windSpeed.Value)),
            "MPH" or "MILES_PER_HOUR" => (MilesPerHourToKilometersPerHour(windSpeed.Value), windSpeed.Value),
            "MPS" or "METERS_PER_SECOND" => (MetersPerSecondToKilometersPerHour(windSpeed.Value), MetersPerSecondToMilesPerHour(windSpeed.Value)),
            _ => (MetersPerSecondToKilometersPerHour(windSpeed.Value), MetersPerSecondToMilesPerHour(windSpeed.Value)) // Default assume m/s
        };
    }
}