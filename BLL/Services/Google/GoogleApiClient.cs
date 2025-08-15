using System.Web;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class GoogleApiClient
{
    private const string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public GoogleApiClient(HttpClient httpClient, string apiKey, ILogger logger)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _logger = logger;
    }

    public async Task<string> FetchGeocodingDataAsync(string query, CancellationToken ct)
    {
        var url = BuildAddressSearchUrl(query);

        try
        {
            var response = await _httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching geocoding data for query: {Query}", query);
            throw new InvalidOperationException($"Failed to fetch geocoding data for query: {query}", ex);
        }
    }

    private string BuildAddressSearchUrl(string query)
    {
        var encodedQuery = HttpUtility.UrlEncode(query);
        return $"{BaseUrl}?address={encodedQuery}&key={_apiKey}";
    }
}