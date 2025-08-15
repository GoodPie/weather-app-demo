using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index(nameof(LocationId))]
[Index(nameof(FetchedAt))]
public class WeatherData : TimeStampedEntity
{
    public int Id { get; set; }
    
    // Foreign key to Location
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
    
    // Weather API response data
    [Required]
    public required string ApiResponse { get; set; }
    
    // Timestamp when this weather data was fetched from the API
    public DateTime FetchedAt { get; set; }
    
    // Weather summary for quick access (extracted from API response)
    [MaxLength(200)]
    public string? Summary { get; set; }
    
    // Temperature data (from temperature.degrees)
    public double? Temperature { get; set; }
    
    // Temperature unit (CELSIUS/FAHRENHEIT)
    [MaxLength(20)]
    public string? TemperatureUnit { get; set; }
    
    // Feels like temperature (from feelsLikeTemperature.degrees)
    public double? FeelsLikeTemperature { get; set; }
    
    // Humidity percentage (from relativeHumidity)
    public double? Humidity { get; set; }
    
    // Weather condition description (from weatherCondition.description.text)
    [MaxLength(100)]
    public string? Condition { get; set; }
    
    // Weather condition type (from weatherCondition.type)
    [MaxLength(50)]
    public string? ConditionType { get; set; }
    
    // Weather icon URI (from weatherCondition.iconBaseUri)
    [MaxLength(500)]
    public string? IconUri { get; set; }
    
    // UV Index
    public double? UvIndex { get; set; }
    
    // Wind speed value
    public double? WindSpeed { get; set; }
    
    // Wind speed unit
    [MaxLength(20)]
    public string? WindSpeedUnit { get; set; }
    
    // Wind direction in degrees
    public double? WindDirection { get; set; }
    
    // Is it daytime (from isDaytime)
    public bool? IsDaytime { get; set; }
    
    // TTL for caching - weather data expires after this time
    public DateTime ExpiresAt { get; set; }
}