using BLL.Services.Contracts;
using DAL.Dtos;
using DAL.Dtos.Location;
using DAL.Repository.Contracts;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class LocationService(ILocationRepository locationRepository, IGeocodingService geocodingService, ILogger<LocationService> logger)
    : ILocationService
{
    private readonly ILogger<LocationService> _logger = logger;
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

    public async Task<ServiceResponse<SearchLocationResponseDto>> FindLocationByIdAsync(int id)
    {
        var response = new ServiceResponse<SearchLocationResponseDto>();

        try
        {
            var location = await locationRepository.FindLocationByIdAsync(id);
            response.Data = SearchLocationResponseDto.MapToDto(location);
            response.Success = true;
            response.Message = "Location found";
        }
        catch (ArgumentException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding location by ID: {Id}", id);
            response.Success = false;
            response.Message = "An error occurred while finding the location";
        }

        return response;
    }

    public async Task<ServiceResponse<List<SearchLocationResponseDto>>> FindLocationByGeolocationAsync(double latitude,
        double longitude)
    {
        var response = new ServiceResponse<List<SearchLocationResponseDto>>();

        try
        {
            var locations = await locationRepository.FindLocationByGeolocationAsync(latitude, longitude);
            var mappedLocations = locations.Select(SearchLocationResponseDto.MapToDto).ToList();

            response.Data = mappedLocations;
            response.Success = true;
            response.Message = locations.Count != 0 ? "Locations found" : "No locations found for the specified coordinates";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding locations by geolocation: {Latitude}, {Longitude}", latitude, longitude);
            response.Success = false;
            response.Message = "An error occurred while finding locations";
        }

        return response;
    }
}