using BLL.Services.Contracts;
using DAL.Models;
using DAL.Repository.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Google;

public class GoogleGeocodingService : IGeocodingService
{
    private readonly GoogleApiClient _apiClient;
    private readonly GoogleGeocodingDataPersistence _dataPersistence;
    private readonly ILogger<GoogleGeocodingService> _logger;
    private readonly GoogleGeocodingResponseParser _responseParser;

    public GoogleGeocodingService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GoogleGeocodingService> logger,
        ILocationRepository locationRepository,
        IGeoCodeSearchRepository geoCodeSearchRepository)
    {
        _logger = logger;

        var apiKey = configuration["GoogleMaps:ApiKey"]
                     ?? throw new InvalidOperationException("Google Maps API key not found in configuration");

        _apiClient = new GoogleApiClient(httpClient, apiKey, logger);
        _responseParser = new GoogleGeocodingResponseParser();
        _dataPersistence = new GoogleGeocodingDataPersistence(
            locationRepository,
            geoCodeSearchRepository,
            logger);
    }

    public async Task<List<Location>> SearchLocationsAsync(string query, CancellationToken ct = default)
    {
        ValidateQuery(query);

        var isNewSearch = await _dataPersistence.IsNewSearchAsync(query, ct);
        if (isNewSearch)
        {
            _logger.LogInformation("Query '{Query}' not found in cache, calling geocoding API", query);
            await GeocodeAndStoreAsync(query, ct);
        }

        var locations = await _dataPersistence.GetLocationsAsync(query);
        _logger.LogInformation("Found {Count} locations for query: {Query}", locations.Count, query);

        return locations.ToList();
    }

    private static void ValidateQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Search query cannot be empty", nameof(query));
    }

    private async Task GeocodeAndStoreAsync(string query, CancellationToken ct)
    {
        var json = await _apiClient.FetchGeocodingDataAsync(query, ct);
        var response = _responseParser.Parse(json);

        await _dataPersistence.SaveSearchResultsAsync(query, response, ct);
    }
}