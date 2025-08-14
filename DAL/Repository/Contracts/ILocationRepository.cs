using DAL.Models;

namespace DAL.Repository.Contracts;

public interface ILocationRepository
{
    /// <summary>
    ///     Finds the location by its city name
    /// </summary>
    /// <param name="cityName"></param>
    /// <returns>Models.Location</returns>
    Task<ICollection<Location>> FindLocationByCityAsync(string cityName);

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
        List<(string city, double lat, double lng, string? formattedAddress, string country, string? iso2)>
            geocodingResults);
}