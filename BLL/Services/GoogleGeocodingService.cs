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

    /// <summary>
    ///     Validates the query string to ensure it is not null or empty.
    /// </summary>
    /// <param name="query">Query</param>
    /// <exception cref="ArgumentException"></exception>
    private static void ValidateQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Search query cannot be empty", nameof(query));
    }

    /// <summary>
    ///     Attempts to geocode the provided query using the Google Maps API.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancelToken"></param>
    private async Task GeocodeAndStoreAsync(string query, CancellationToken cancelToken)
    {
        var url = BuildGeocodingUrl(query);
        var json = await FetchGeocodingData(url, cancelToken);
        var response = ParseGeocodingResponse(json);

        if (response.Results.Count > 0)
            // Save API results to Location table
            await SaveApiResultsToDatabase(response, query);

        // Record that we've searched this query (even if no results)
        await _geoCodeSearchRepository.AddSearchAsync(query, response.Results.Count, cancelToken);
    }

    /// <summary>
    ///     Builds the URL for the Google Geocoding API request.
    ///     Encodes the query to ensure it is safe for use in a URL.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    private string BuildGeocodingUrl(string query)
    {
        var encodedQuery = HttpUtility.UrlEncode(query);
        return $"{BaseUrl}?address={encodedQuery}&key={_apiKey}";
    }

    /// <summary>
    ///     Attempts to fetch geocoding data from the Google Maps API.
    ///     Logs an error if the request fails and throws an InvalidOperationException.
    /// </summary>
    /// <param name="url">URL for Google API</param>
    /// <param name="cancelToken">Token to cancel the request</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task<string> FetchGeocodingData(string url, CancellationToken cancelToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(url, cancelToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancelToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching geocoding data from URL: {Url}", url);
            throw new InvalidOperationException("Failed to fetch geocoding data", ex);
        }
    }

    /// <summary>
    ///     Parse the JSON response from the Google Geocoding API into a DTO.
    /// </summary>
    /// <param name="json">JSON response from the Google APi</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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


    /// <summary>
    ///     Saves the geocoding results to the database
    /// </summary>
    /// <param name="response">Parsed response</param>
    /// <param name="query">Query string</param>
    private async Task SaveApiResultsToDatabase(GoogleGeocodingDto response, string query)
    {
        if (response.Results.Count == 0) return;

        var geocodingResults = response.Results
            .Select(googleResponse =>
            {
                var countryInfo = ExtractCountryInfo(googleResponse);
                var cityName = ExtractCityName(googleResponse, query);
                return (
                    city: cityName,
                    lat: googleResponse.Geometry.Location.Lat,
                    lng: googleResponse.Geometry.Location.Lng,
                    formattedAddress: googleResponse.FormattedAddress,
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

    /// <summary>
    ///     Extract country information from the geocoding result.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
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