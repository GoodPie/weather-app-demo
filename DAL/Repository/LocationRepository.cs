using DAL.Dtos.Location;
using DAL.Models;
using DAL.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class LocationRepository(WeatherDbContext context) : ILocationRepository
{
    public async Task<ICollection<Location>> FindLocationByQuery(string query)
    {
        // Break the query into words and search for each word in the city name, splitting by space and comma
        query = query.Trim().ToLower();
        var words = query.Split([" ", ","], StringSplitOptions.RemoveEmptyEntries);

        // Normalize the query to lower case for case-insensitive search
        return await context.Locations
            .Where(p =>
                EF.Functions.Like(p.City, $"%{query}%") ||
                EF.Functions.Like(p.Country, $"%{query}%") ||
                words.Contains(p.City))
            .OrderBy(p => p.City)
            .ThenBy(p => p.Country)
            .Take(10) // Limit to 10 results
            .ToListAsync();
    }

    public async Task<ICollection<Location>> SearchLocationsAsync(string query, int limit = 10)
    {
        query = query.Trim().ToLower();

        return await context.Locations
            .Where(l =>
                EF.Functions.Like(l.City, $"%{query}%") ||
                EF.Functions.Like(l.Country, $"%{query}%"))
            .OrderBy(l => l.City)
            .ThenBy(l => l.Country)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<ICollection<Location>> FindLocationByGeolocationAsync(double latitude, double longitude)
    {
        // Find locations within a reasonable distance (about 0.1 degrees ~ 11km)
        const double tolerance = 0.1;
        
        return await context.Locations
            .Where(location => 
                Math.Abs(location.Latitude - latitude) <= tolerance &&
                Math.Abs(location.Longitude - longitude) <= tolerance)
            .OrderBy(location => 
                Math.Abs(location.Latitude - latitude) + Math.Abs(location.Longitude - longitude)) // Simple distance approximation
            .Take(10)
            .ToListAsync();
    }

    public async Task<Location> FindLocationByIdAsync(int id)
    {
        var location = await context.Locations.FindAsync(id);
        return location ?? throw new ArgumentException($"Location with ID {id} not found", nameof(id));
    }


    public async Task<List<Location>> SaveGeocodingResultsAsync(List<GeocodingLocationDto> geocodingResults)
    {
        var savedLocations = new List<Location>();

        foreach (var geocodeResult in geocodingResults)
        {
            var newLocations = geocodingResults.Select(GeocodingLocationDto.MapToModel).ToList();


            // Check if already exists (in case of race conditions)
            // Messy but just to get the project done
            if (await context.Locations.AnyAsync(l =>
                    l.City == geocodeResult.City && l.Country == geocodeResult.Country))
                continue;

            context.Locations.AddRange(newLocations);
            savedLocations.AddRange(newLocations);
        }

        await context.SaveChangesAsync();
        return savedLocations;
    }
}