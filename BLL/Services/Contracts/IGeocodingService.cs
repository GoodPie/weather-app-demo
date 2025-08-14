namespace BLL.Services.Contracts;

public interface IGeocodingService
{
    /// <summary>
    ///     Simple location search:
    ///     1. Check if query has been searched before
    ///     2. If not, geocode via API and store results
    ///     3. Return all locations matching the query from database
    /// </summary>
    /// <param name="query">Search query (e.g. "Morley", "Sydney NSW", "London UK")</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of locations matching the query</returns>
    Task<List<DAL.Models.Location>> SearchLocationsAsync(string query, CancellationToken ct = default);
}