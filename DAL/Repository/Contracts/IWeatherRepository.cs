using DAL.Models;

namespace DAL.Repository.Contracts;

public interface IWeatherRepository
{
    /// <summary>
    /// Gets cached weather data for a location if it exists and hasn't expired
    /// </summary>
    /// <param name="locationId">The location ID to get weather for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>WeatherData if cached and valid, null otherwise</returns>
    Task<WeatherData?> GetCachedWeatherAsync(int locationId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Saves weather data fetched from the API
    /// </summary>
    /// <param name="weatherData">Weather data to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Saved weather data</returns>
    Task<WeatherData> SaveWeatherDataAsync(WeatherData weatherData, CancellationToken cancellationToken = default);
    
    
    /// <summary>
    /// Checks if weather data exists for a location (regardless of expiration)
    /// </summary>
    /// <param name="locationId">The location ID to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if weather data exists, false otherwise</returns>
    Task<bool> HasWeatherDataAsync(int locationId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the most recent weather data for a location (even if expired)
    /// </summary>
    /// <param name="locationId">The location ID to get weather for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Most recent weather data or null if none exists</returns>
    Task<WeatherData?> GetLatestWeatherAsync(int locationId, CancellationToken cancellationToken = default);
    
}