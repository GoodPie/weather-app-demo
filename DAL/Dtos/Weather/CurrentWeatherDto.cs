namespace DAL.Dtos.Weather;

public class CurrentWeatherDto
{
    public string AsOf { get; set; } = null!; // ISO timestamp
    public double TempC { get; set; }
    public double TempF { get; set; }
    public double? FeelsLikeC { get; set; }
    public double? FeelsLikeF { get; set; }
    public string? ConditionCode { get; set; }
    public string? ConditionText { get; set; }
    public double? WindKph { get; set; }
    public double? WindMph { get; set; }
    public string? WindDir { get; set; }
    public double? Humidity { get; set; }
    public double? PressureMb { get; set; }
    public double? UvIndex { get; set; }
    public double? VisibilityKm { get; set; }
    public string? Sunrise { get; set; }
    public string? Sunset { get; set; }
    public bool? IsDay { get; set; }
}