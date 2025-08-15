using System.Web;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Google;

public class GoogleApiClient(HttpClient httpClient, string apiKey, string baseUrl, ILogger logger)
{
    private readonly string _baseUrl = baseUrl;

    public async Task<string> FetchGeocodingDataAsync(string query, CancellationToken ct)
    {
        var url = BuildAddressSearchUrl(query);

        try
        {
            var response = await httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(ct);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error fetching geocoding data for query: {Query}", query);
            throw new InvalidOperationException($"Failed to fetch geocoding data for query: {query}", ex);
        }
    }

    public async Task<string> FetchWeatherDataAsync(double latitude, double longitude, string weatherBaseUrl,
        CancellationToken ct)
    {
        var url = BuildWeatherUrl(latitude, longitude, weatherBaseUrl);

        try
        {
            var response = await httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(ct);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error fetching weather data for coordinates: {Latitude}, {Longitude}", latitude,
                longitude);
            throw new InvalidOperationException(
                $"Failed to fetch weather data for coordinates: {latitude}, {longitude}", ex);
        }
    }

    private string BuildAddressSearchUrl(string query)
    {
        var encodedQuery = HttpUtility.UrlEncode(query);
        return $"{_baseUrl}?address={encodedQuery}&key={apiKey}";
    }

    private string BuildWeatherUrl(double latitude, double longitude, string weatherBaseUrl)
    {
        // Google Weather API endpoint format: https://weather.googleapis.com/v1/forecast/days:lookup
        // with location parameter and API key
        return
            $"{weatherBaseUrl}currentConditions:lookup?location.latitude={latitude}&location.longitude={longitude}&key={apiKey}";
    }
}