using DAL.Dtos.Google;
using DAL.Dtos.Location;
using DAL.Models;
using DAL.Repository.Contracts;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Google;

public class GoogleGeocodingDataPersistence(
    ILocationRepository locationRepository,
    IGeoCodeSearchRepository geoCodeSearchRepository,
    ILogger logger)
{
    public async Task<bool> IsNewSearchAsync(string query, CancellationToken cancelToken)
    {
        return !await geoCodeSearchRepository.HasBeenSearchedAsync(query, cancelToken);
    }

    public async Task<ICollection<Location>> GetLocationsAsync(string query)
    {
        return await locationRepository.SearchLocationsAsync(query);
    }

    public async Task SaveSearchResultsAsync(string query, GoogleGeocodingDto response,
        CancellationToken cancelToken)
    {
        if (response.Results.Count > 0)
        {
            var geocodingLocations = ConvertToLocationData(response.Results);
            await SaveLocationsAsync(geocodingLocations, query);
        }

        await geoCodeSearchRepository.AddSearchAsync(query, response.Results.Count, cancelToken);
    }

    public async Task SaveLocationsAsync(List<GeocodingLocationDto> geocodingLocations, string query)
    {
        if (geocodingLocations.Count == 0) return;

        await locationRepository.SaveGeocodingResultsAsync(geocodingLocations);
        logger.LogInformation("Saved {Count} geocoding results for query: {Query}", geocodingLocations.Count, query);
    }

    private List<GeocodingLocationDto> ConvertToLocationData(List<GoogleGeocodingResultDto> results)
    {
        var locations = new List<GeocodingLocationDto>();
        if (results.Count == 0) return locations;

        foreach (var result in results)
            try
            {
                var locationData = new GoogleAddressComponentExtractor(result).ExtractLocationData();
                locations.Add(locationData);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning("No location data extracted for result: {warning}", ex.Message);
            }


        return locations;
    }
}