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
    public DbSet<GeocodeSearch> GeocodeSearches { get; set; }

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

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is TimeStampedEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added) ((TimeStampedEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
            ((TimeStampedEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is TimeStampedEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added) ((TimeStampedEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
            ((TimeStampedEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
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