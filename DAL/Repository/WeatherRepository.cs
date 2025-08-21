using DAL.Models;
using DAL.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class WeatherRepository(WeatherDbContext context) : IWeatherRepository
{
    public async Task<WeatherData?> GetCachedWeatherAsync(int locationId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await context.WeatherData
            .Include(w => w.Location)
            .Where(w => w.LocationId == locationId && w.ExpiresAt > now)
            .OrderByDescending(w => w.FetchedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WeatherData> SaveWeatherDataAsync(WeatherData weatherData,
        CancellationToken cancellationToken = default)
    {
        // Set fetch timestamp if not already set
        if (weatherData.FetchedAt == default) weatherData.FetchedAt = DateTime.UtcNow;

        // Set expiration if not already set (default 1 hour)
        if (weatherData.ExpiresAt == default) weatherData.ExpiresAt = DateTime.UtcNow.AddHours(1);

        context.WeatherData.Add(weatherData);
        await context.SaveChangesAsync(cancellationToken);

        return weatherData;
    }


    public async Task<bool> HasWeatherDataAsync(int locationId, CancellationToken cancellationToken = default)
    {
        return await context.WeatherData
            .AnyAsync(w => w.LocationId == locationId, cancellationToken);
    }

    public async Task<WeatherData?> GetLatestWeatherAsync(int locationId, CancellationToken cancellationToken = default)
    {
        return await context.WeatherData
            .Include(w => w.Location)
            .Where(w => w.LocationId == locationId)
            .OrderByDescending(w => w.FetchedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}