using DAL.Dtos;
using DAL.Dtos.Weather;

namespace BLL.Services.Contracts;

public interface IWeatherService
{
    /// <summary>
    /// Gets current weather data for a specific location by its ID
    /// </summary>
    /// <param name="locationId">The location ID to get weather for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ServiceResponse containing current weather data</returns>
    Task<ServiceResponse<CurrentWeatherDto>> GetCurrentWeatherByLocationIdAsync(int locationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets hourly forecast for a specific location
    /// </summary>
    /// <param name="locationId">The location ID to get forecast for</param>
    /// <param name="hours">Number of hours to forecast (max 48)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ServiceResponse containing hourly forecast data</returns>
    Task<ServiceResponse<List<HourlyForecastDto>>> GetHourlyForecastByLocationIdAsync(int locationId, int hours = 24, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets daily forecast for a specific location
    /// </summary>
    /// <param name="locationId">The location ID to get forecast for</param>
    /// <param name="days">Number of days to forecast (max 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ServiceResponse containing daily forecast data</returns>
    Task<ServiceResponse<List<DailyForecastDto>>> GetDailyForecastByLocationIdAsync(int locationId, int days = 7, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets complete weather bundle (current + forecasts) for a specific location
    /// </summary>
    /// <param name="locationId">The location ID to get weather for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ServiceResponse containing complete weather bundle</returns>
    Task<ServiceResponse<WeatherBundleDto>> GetWeatherBundleByLocationIdAsync(int locationId, CancellationToken cancellationToken = default);
}