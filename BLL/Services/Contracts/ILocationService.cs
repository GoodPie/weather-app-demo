using DAL.Dtos;
using DAL.Dtos.Location;

namespace BLL.Services.Contracts;

public interface ILocationService
{
    /// <summary>
    ///     Finds the location by its city name
    /// </summary>
    /// <param name="cityName"></param>
    /// <returns>DAL.Dtos.SearchLocationResponseDto</returns>
    Task<ServiceResponse<List<SearchLocationResponseDto>>> FindLocationByCityAsync(string cityName);

    /// <summary>
    ///     Finds the location by its geolocation (latitude and longitude).
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns>DAL.Dtos.SearchLocationResponseDto</returns>
    Task<ServiceResponse<List<SearchLocationResponseDto>>> FindLocationByGeolocationAsync(double latitude,
        double longitude);

    /// <summary>
    ///     Finds the location by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>DAL.Dtos.SearchLocationResponseDto</returns>
    Task<ServiceResponse<SearchLocationResponseDto>> FindLocationByIdAsync(int id);
}