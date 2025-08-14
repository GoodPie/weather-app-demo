using DAL.Models;
using DAL.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class LocationRepository(WeatherDbContext context) : ILocationRepository
{
    public async Task<ICollection<Location>> FindLocationByCityAsync(string cityName)
    {
        return await context.Locations
            .Where(location => location.City
                .Any((city) => EF.Functions.Like(cityName, $"%{city}%"))
            )
            .ToListAsync();
    }

    public Task<ICollection<Location>> FindLocationByGeolocationAsync(double latitude, double longitude)
    {
        throw new NotImplementedException();
    }

    public Task<Location> FindLocationByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}