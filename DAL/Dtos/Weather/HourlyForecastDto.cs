namespace DAL.Dtos.Weather;

public class HourlyForecastDto
{
    public string Time { get; set; } = null!; // ISO timestamp
    public double TempC { get; set; }
    public double TempF { get; set; }
    public string? ConditionCode { get; set; }
    public double? PrecipMm { get; set; }
    public double? Pop { get; set; } // probability of precipitation (0-100)
    public double? WindKph { get; set; }
    public double? WindMph { get; set; }
    public string? WindDir { get; set; }
}