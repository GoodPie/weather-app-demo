using System.Text.Json;
using System.Web;
using BLL.Services.Contracts;
using DAL.Dtos.Google;
using DAL.Models;
using DAL.Repository.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class GoogleGeocodingService : IGeocodingService
{
    // Base URL for Google Geocoding API
    private const string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";
    private readonly string _apiKey;
    private readonly IGeoCodeSearchRepository _geoCodeSearchRepository;
    private readonly HttpClient _httpClient;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<GoogleGeocodingService> _logger;

    public GoogleGeocodingService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GoogleGeocodingService> logger,
        ILocationRepository locationRepository,
        IGeoCodeSearchRepository geoCodeSearchRepository)
    {
        _httpClient = httpClient;
        _logger = logger;
        _locationRepository = locationRepository;
        _geoCodeSearchRepository = geoCodeSearchRepository;
        _apiKey = configuration["GoogleMaps:ApiKey"] ??
                  throw new InvalidOperationException("Google Maps API key not found in configuration");
    }

    public async Task<List<Location>> SearchLocationsAsync(string query, CancellationToken ct = default)
    {
        ValidateQuery(query);

        if (!await _geoCodeSearchRepository.HasBeenSearchedAsync(query, ct))
        {
            _logger.LogInformation("Query '{Query}' not found in cache, calling geocoding API", query);
            await GeocodeAndStoreAsync(query, ct);
        }

        var locations = await _locationRepository.SearchLocationsAsync(query);

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
        var url = BuildGeocodingUrl(query);
        var json = await FetchGeocodingData(url, ct);
        var response = ParseGeocodingResponse(json);

        if (response.Results.Count > 0)
            // Save API results to Location table
            await SaveApiResultsToDatabase(response, query);

        // Record that we've searched this query (even if no results)
        await _geoCodeSearchRepository.AddSearchAsync(query, response.Results.Count, ct);
    }

    private string BuildGeocodingUrl(string query)
    {
        var encodedQuery = HttpUtility.UrlEncode(query);
        return $"{BaseUrl}?address={encodedQuery}&key={_apiKey}";
    }

    private async Task<string> FetchGeocodingData(string url, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct);
    }

    private static GoogleGeocodingDto ParseGeocodingResponse(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        var result = JsonSerializer.Deserialize<GoogleGeocodingDto>(json, options);
        return result ?? throw new InvalidOperationException("Failed to parse geocoding response");
    }


    private async Task SaveApiResultsToDatabase(GoogleGeocodingDto response, string query)
    {
        if (response.Results.Count == 0) return;

        var geocodingResults = response.Results
            .Select(r =>
            {
                var countryInfo = ExtractCountryInfo(r);
                var cityName = ExtractCityName(r, query);
                return (
                    city: cityName,
                    lat: r.Geometry.Location.Lat,
                    lng: r.Geometry.Location.Lng,
                    formattedAddress: (string?)r.FormattedAddress,
                    countryInfo.country,
                    countryInfo.iso2
                );
            })
            .ToList();

        if (geocodingResults.Count > 0)
        {
            await _locationRepository.SaveGeocodingResultsAsync(geocodingResults);
            _logger.LogInformation("Saved {Count} geocoding results for query: {Query}", geocodingResults.Count, query);
        }
    }

    private static (string country, string? iso2) ExtractCountryInfo(GoogleGeocodingResultDto result)
    {
        var countryComponent = result.AddressComponents
            .FirstOrDefault(ac => ac.Types.Contains("country"));

        // TODO: Investigate the best way to handle this later
        if (countryComponent == null) return ("Unknown", null);

        var iso2 = countryComponent.ShortName;
        return (countryComponent.LongName, iso2);
    }

    /// <summary>
    ///     Extracts the city name from the geocoding result.
    ///     It first checks for the most specific locality, then falls back to administrative area level
    /// </summary>
    /// <param name="result"></param>
    /// <param name="fallbackCityName"></param>
    /// <returns></returns>
    private static string ExtractCityName(GoogleGeocodingResultDto result, string fallbackCityName)
    {
        // Look for locality component first (most specific)
        var localityComponent = result.AddressComponents
            .FirstOrDefault(ac => ac.Types.Contains("locality"));

        if (localityComponent != null && !string.IsNullOrWhiteSpace(localityComponent.LongName))
            return localityComponent.LongName;

        // Fallback to administrative_area_level_2 (county/district level)
        var adminLevel2Component = result.AddressComponents
            .FirstOrDefault(ac => ac.Types.Contains("administrative_area_level_2"));

        if (adminLevel2Component != null && !string.IsNullOrWhiteSpace(adminLevel2Component.LongName))
            return adminLevel2Component.LongName;

        // If no locality found, fallback to the original search term
        return fallbackCityName;
    }
}