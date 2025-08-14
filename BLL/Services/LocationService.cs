using BLL.Services.Contracts;
using DAL.Dtos;
using DAL.Dtos.Location;
using DAL.Repository.Contracts;

namespace BLL.Services;

public class LocationService(ILocationRepository locationRepository) : ILocationService
{
    public async Task<ServiceResponse<List<SearchLocationResponseDto>>> FindLocationByCityAsync(string cityName)
    {
        ServiceResponse<List<SearchLocationResponseDto>> response = new();

        var locations = await locationRepository.FindLocationByCityAsync(cityName);
        var mappedLocations = locations.Select(SearchLocationResponseDto.MapToDto).ToList();

        // Successfully found locations
        response.Data = mappedLocations;
        response.Success = true;
        response.Message = locations.Any() ? "Locations found" : "No locations found for the specified city";
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