using BLL.Services.Contracts;
using DAL.Dtos;
using DAL.Dtos.Location;
using DAL.Repository.Contracts;

namespace BLL.Services;

public class LocationService(ILocationRepository locationRepository) : ILocationService
{

    public async Task<ServiceResponse<ICollection<SearchLocationResponseDto>>> FindLocationByCityAsync(string cityName)
    {
        // Find the location from the DB
        var locations = locationRepository.FindLocationByCityAsync(cityName);
        
        // Map the locations to the response DTO
        var mappedLocations = locations.Result.Select(SearchLocationResponseDto.MapToDto).ToList();
        
        // Return the response
        return new ServiceResponse<ICollection<SearchLocationResponseDto>>(mappedLocations);
    }

    public Task<ServiceResponse<ICollection<SearchLocationResponseDto>>> FindLocationByGeolocationAsync(double latitude, double longitude)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<SearchLocationResponseDto>> FindLocationByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}