using System.Text.Json;
using System.Web;
using BLL.Services.Contracts;
using DAL.Repository.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class GoogleGeocodingService : IGeocodingService
{
    // Base URL for Google Geocoding API
    private const string _baseUrl = "https://maps.googleapis.com/maps/api/geocode/json";
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<GoogleGeocodingService> _logger;

    public GoogleGeocodingService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GoogleGeocodingService> logger,
        ILocationRepository locationRepository)
    {
        _httpClient = httpClient;
        _logger = logger;
        _locationRepository = locationRepository;
        _apiKey = configuration["GoogleMaps:ApiKey"] ??
                  throw new InvalidOperationException("Google Maps API key not found in configuration");
    }

    public async Task<(double lat, double lng)> GetLatLngByCityAsync(string city, CancellationToken ct = default)
    {
        ValidateCity(city);

        var url = BuildGeocodingUrl(city);
        _logger.LogInformation("Geocoding city: {City}", city);

        var json = await FetchGeocodingData(url, ct);
        var result = ParseGeocodingResponse(json);

        // Process and save all results to database
        await SaveAllResultsToDatabase(result, city);

        var coordinates = ExtractCoordinates(result, city);
        _logger.LogInformation("Found coordinates for {City}: {Lat}, {Lng}", city, coordinates.lat, coordinates.lng);
        return coordinates;
    }

    private static void ValidateCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
    }

    private string BuildGeocodingUrl(string city)
    {
        var encodedCity = HttpUtility.UrlEncode(city);
        return $"{_baseUrl}?address={encodedCity}&key={_apiKey}";
    }

    private async Task<string> FetchGeocodingData(string url, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct);
    }

    private static GeocodingResponse ParseGeocodingResponse(string json)
    {
        var result = JsonSerializer.Deserialize<GeocodingResponse>(json);
        return result ?? throw new InvalidOperationException("Failed to parse geocoding response");
    }

    private async Task SaveAllResultsToDatabase(GeocodingResponse response, string city)
    {
        if (response.Results.Count == 0)
            return;

        var geocodingResults = response.Results
            .Where(r => r.Geometry?.Location != null)
            .Select(r =>
            {
                var countryInfo = ExtractCountryInfo(r);
                return (
                    city,
                    lat: r.Geometry.Location.Lat,
                    lng: r.Geometry.Location.Lng,
                    formattedAddress: (string?)r.FormattedAddress,
                    countryInfo.country,
                    countryInfo.iso2
                );
            })
            .ToList();

        if (geocodingResults.Count == 0) return;

        await _locationRepository.SaveGeocodingResultsAsync(geocodingResults);
        _logger.LogInformation("Saved {Count} geocoding results for city: {City}", geocodingResults.Count, city);
    }

    private static (double lat, double lng) ExtractCoordinates(GeocodingResponse response, string city)
    {
        var location = response.Results?.FirstOrDefault()?.Geometry?.Location;
        if (location == null)
            throw new InvalidOperationException($"No location found for city: {city}");

        return (location.Lat, location.Lng);
    }

    private static (string country, string? iso2) ExtractCountryInfo(GeocodingResult result)
    {
        var countryComponent = result.AddressComponents
            .FirstOrDefault(ac => ac.Types.Contains("country"));

        if (countryComponent == null)
            return ("Unknown", null);

        var iso2 = countryComponent.ShortName;

        return (countryComponent.LongName, iso2);
    }
}

// Internal response models - only used by this service
// These are mapped to the JSON structure returned by Google Geocoding API
// We will look at best practice later, but for now, this is sufficient
internal class GeocodingResponse
{
    public string Status { get; set; } = "";
    public List<GeocodingResult> Results { get; set; } = new();
}

internal abstract class GeocodingResult
{
    public Geometry Geometry { get; set; } = new();
    public string FormattedAddress { get; set; } = "";
    public List<AddressComponent> AddressComponents { get; set; } = new();
}

internal class Geometry
{
    public Location Location { get; set; } = new();
}

internal class Location
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

internal abstract class AddressComponent
{
    public string LongName { get; set; } = "";
    public string ShortName { get; set; } = "";
    public List<string> Types { get; set; } = new();
}