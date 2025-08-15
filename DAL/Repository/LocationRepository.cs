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

    public Task<ICollection<Location>> FindLocationByGeolocationAsync(double latitude, double longitude)
    {
        throw new NotImplementedException();
    }

    public Task<Location> FindLocationByIdAsync(int id)
    {
        throw new NotImplementedException();
    }


    public async Task<List<Location>> SaveGeocodingResultsAsync(List<GeocodingLocationDto> geocodingResults)
    {
        var savedLocations = new List<Location>();

        foreach (var geocodeResult in geocodingResults)
        {
            var newLocation = GeocodingLocationDto.MapToModel(geocodeResult);
            context.Locations.Add(newLocation);
            savedLocations.Add(newLocation);
        }

        await context.SaveChangesAsync();
        return savedLocations;
    }
}