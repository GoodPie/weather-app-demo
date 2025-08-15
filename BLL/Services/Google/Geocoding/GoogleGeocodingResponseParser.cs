using System.Text.Json;
using DAL.Dtos.Google;

namespace BLL.Services.Google.Geocoding;

public class GoogleGeocodingResponseParser
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public GoogleGeocodingDto Parse(string json)
    {
        return JsonSerializer.Deserialize<GoogleGeocodingDto>(json, _jsonOptions)
               ?? throw new InvalidOperationException("Failed to parse geocoding response");
    }
}