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

    private string BuildAddressSearchUrl(string query)
    {
        var encodedQuery = HttpUtility.UrlEncode(query);
        return $"{_baseUrl}?address={encodedQuery}&key={apiKey}";
    }
}