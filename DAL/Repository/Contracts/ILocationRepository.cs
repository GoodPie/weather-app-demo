using DAL.Dtos.Location;
using DAL.Models;

namespace DAL.Repository.Contracts;

public interface ILocationRepository
{
    /// <summary>
    ///     Finds the location by its city name
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Models.Location</returns>
    Task<ICollection<Location>> FindLocationByQuery(string query);

    /// <summary>
    ///     Searches locations by query string across city, country, and formatted address
    /// </summary>
    /// <param name="query">Search query that can match city name, country, or parts of address</param>
    /// <param name="limit">Maximum number of results to return</param>
    /// <returns>Collection of matching locations</returns>
    Task<ICollection<Location>> SearchLocationsAsync(string query, int limit = 10);

    /// <summary>
    ///     Find Location by its geolocation (latitude and longitude)
    ///     This is useful for finding locations that are close to a given point.
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns>Models.Location</returns>
    Task<ICollection<Location>> FindLocationByGeolocationAsync(double latitude, double longitude);

    /// <summary>
    ///     Finds the location by its unique identifier
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Models.Location</returns>
    Task<Location> FindLocationByIdAsync(int id);


    /// <summary>
    ///     Saves multiple geocoding results to the database
    /// </summary>
    /// <param name="geocodingResults"></param>
    /// <returns>List of saved locations</returns>
    Task<List<Location>> SaveGeocodingResultsAsync(
        List<GeocodingLocationDto> geocodingResults);
}