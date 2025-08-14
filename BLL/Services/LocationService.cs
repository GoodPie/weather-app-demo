using BLL.Services.Contracts;
using DAL.Dtos;
using DAL.Dtos.Location;
using DAL.Repository.Contracts;

namespace BLL.Services;

public class LocationService(ILocationRepository locationRepository, IGeocodingService geocodingService)
    : ILocationService
{
    public async Task<ServiceResponse<List<SearchLocationResponseDto>>> FindLocationByCityAsync(string cityName)
    {
        ServiceResponse<List<SearchLocationResponseDto>> response = new();

        // Geocoding service call to find coordinates by city name only if it hasn't been geocoded before
        await geocodingService.SearchLocationsAsync(cityName);

        var locations = await locationRepository.FindLocationByQuery(cityName);
        var mappedLocations = locations.Select(SearchLocationResponseDto.MapToDto).ToList();

        // Successfully found locations
        response.Data = mappedLocations;
        response.Success = true;
        response.Message = locations.Count != 0 ? "Locations found" : "No locations found for the specified city";
        return response;
    }

    public Task<ServiceResponse<SearchLocationResponseDto>> FindLocationByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<List<SearchLocationResponseDto>>> FindLocationByGeolocationAsync(double latitude,
        double longitude)
    {
        throw new NotImplementedException();
    }
}