namespace DAL.Seeds;

public interface IDataSeed
{
    Task SeedAsync(WeatherDbContext context, CancellationToken cancellationToken = default);
}