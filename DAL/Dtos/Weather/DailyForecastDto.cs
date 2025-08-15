namespace DAL.Dtos.Weather;

public class DailyForecastDto
{
    public string Date { get; set; } = null!; // ISO date (YYYY-MM-DD)
    public double MinTempC { get; set; }
    public double MaxTempC { get; set; }
    public double MinTempF { get; set; }
    public double MaxTempF { get; set; }
    public string? ConditionCode { get; set; }
    public string? Sunrise { get; set; }
    public string? Sunset { get; set; }
    public double? PrecipMm { get; set; }
    public double? Pop { get; set; } // probability of precipitation (0-100)
}