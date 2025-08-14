using DAL.Models;
using DAL.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class GeoCodeSearchRepository(WeatherDbContext context) : IGeoCodeSearchRepository
{
    public async Task<bool> HasBeenSearchedAsync(string query, CancellationToken cancelToken)
    {
        var normalizedQuery = query.Trim().ToLowerInvariant();
        return await context.GeocodeSearches
            .AnyAsync(gs => gs.SearchTerm.ToLower() == normalizedQuery, cancelToken);
    }

    public async Task AddSearchAsync(string query, int resultCount, CancellationToken cancelToken = default)
    {
        var normalizedCity = query.Trim().ToLowerInvariant();

        // Check if already exists (in case of race conditions)
        var existingSearch = await context.GeocodeSearches
            .FirstOrDefaultAsync(geocodeSearch =>
                geocodeSearch.SearchTerm.ToLower() == normalizedCity, cancelToken);

        if (existingSearch != null)
        {
            // Update existing record
            existingSearch.SearchedAt = DateTime.UtcNow;
            existingSearch.ResultCount = resultCount;
        }
        else
        {
            // Create new record
            var geocodeSearch = new GeocodeSearch
            {
                SearchTerm = normalizedCity,
                SearchedAt = DateTime.UtcNow,
                ResultCount = resultCount
            };
            context.GeocodeSearches.Add(geocodeSearch);
        }

        await context.SaveChangesAsync(cancelToken);
    }
}