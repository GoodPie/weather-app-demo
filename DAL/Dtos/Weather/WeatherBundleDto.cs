namespace DAL.Dtos.Weather;

public class WeatherBundleDto
{
    public CurrentWeatherDto? Current { get; set; }
    public List<HourlyForecastDto> Hourly { get; set; } = new();
    public List<DailyForecastDto> Daily { get; set; } = new();
}