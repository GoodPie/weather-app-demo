using DAL.Models;
using DAL.Seeds;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
        : base(options)
    {
    }

    public WeatherDbContext()
    {
    }

    public DbSet<Location> Locations { get; set; }

    // Configure EF to create a Sqlite database at the specified path, locally
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Always configure seeding
        optionsBuilder
            // UseSqlite is used to create a SQLite database
            // Get value from csproj
            .UseSqlite("Data Source=../DAL/Database/WeatherApp.db;Cache=Shared;")
            .UseSeeding(SeedData)
            .UseAsyncSeeding(SeedDataAsync);
    }

    private static async Task SeedDataAsync(DbContext context, bool isDevelopment,
        CancellationToken cancellationToken = default)
    {
        if (context is not WeatherDbContext weatherContext) return;

        var seeders = new List<IDataSeed>
        {
            new CityLocationsSeed()
        };

        foreach (var seeder in seeders) await seeder.SeedAsync(weatherContext, cancellationToken);
    }

    private static void SeedData(DbContext context, bool isDevelopment)
    {
        if (context is not WeatherDbContext weatherContext) return;

        var seeders = new List<IDataSeed>
        {
            new CityLocationsSeed()
        };

        foreach (var seeder in seeders) seeder.SeedAsync(weatherContext).GetAwaiter().GetResult();
    }
}