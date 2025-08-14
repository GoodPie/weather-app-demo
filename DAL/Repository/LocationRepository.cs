using DAL.Models;
using DAL.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class LocationRepository(WeatherDbContext context) : ILocationRepository
{
    private const float LatitudeTolerance = 0.01f; // ~1km

    public async Task<ICollection<Location>> FindLocationByCityAsync(string cityName)
    {
        return await context.Locations
            .Where(p => EF.Functions.Like(p.City.ToLower(), $"%{cityName.ToLower()}%"))
            .ToListAsync();
    }

    public async Task<ICollection<Location>> SearchLocationsAsync(string query, int limit = 50)
    {
        var normalizedQuery = query.Trim().ToLower();
        
        return await context.Locations
            .Where(l => 
                EF.Functions.Like(l.City.ToLower(), $"%{normalizedQuery}%") ||
                EF.Functions.Like(l.Country.ToLower(), $"%{normalizedQuery}%"))
            .OrderBy(l => l.City)
            .ThenBy(l => l.Country)
            .Take(limit)
            .ToListAsync();
    }

    public Task<ICollection<Location>> FindLocationByGeolocationAsync(double latitude, double longitude)
    {
        throw new NotImplementedException();
    }

    public Task<Location> FindLocationByIdAsync(int id)
    {
        throw new NotImplementedException();
    }


    public async Task<List<Location>> SaveGeocodingResultsAsync(
        List<(string city, double lat, double lng, string? formattedAddress, string country, string? iso2)>
            geocodingResults)
    {
        var savedLocations = new List<Location>();

        foreach (var (city, lat, lng, _, country, iso2) in geocodingResults)
        {
            // Check if location already exists (within tolerance)
            var existingLocation = await context.Locations
                .FirstOrDefaultAsync(l =>
                    Math.Abs(l.Latitude - lat) < LatitudeTolerance &&
                    Math.Abs(l.Longitude - lng) < LatitudeTolerance);

            if (existingLocation != null)
            {
                savedLocations.Add(existingLocation);
                continue;
            }

            // Create new location
            var newLocation = new Location
            {
                City = city,
                Latitude = lat,
                Longitude = lng,
                Country = country,
                Iso2 = iso2
            };

            context.Locations.Add(newLocation);
            savedLocations.Add(newLocation);
        }

        await context.SaveChangesAsync();
        return savedLocations;
    }
}