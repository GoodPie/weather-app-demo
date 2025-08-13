using DAL.Models;
using DAL.Seeds;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class WeatherDbContext : DbContext
{
    private string DatabasePath { get; }

    private static string DatabaseName => Environment.GetEnvironmentVariable("DatabaseName") ?? "WeatherApp.db";

    public DbSet<Location> Locations { get; set; }

    public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
        : base(options)
    {
        DatabasePath = BuildDatabasePath();
    }

    public WeatherDbContext()
    {
        DatabasePath = BuildDatabasePath();
    }

    // Configure EF to create a Sqlite database at the specified path, locally
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only configure if options haven't been set (e.g., in testing scenarios)
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlite($"Data Source={DatabasePath}");
        }
        
        // Always configure seeding
        optionsBuilder
            .UseSeeding(SeedData)
            .UseAsyncSeeding(SeedDataAsync);
    }

    /// <summary>
    /// Builds the path to the SQLite database file in the DAL layer.
    /// </summary>
    /// <returns>Local path to the database in the DAL directory</returns>
    public static string BuildDatabasePath()
    {
        // Get the DAL project directory
        var dalDirectory = AppDomain.CurrentDomain.BaseDirectory;
        
        // For development, navigate to the DAL folder relative to the executing assembly
        var projectRoot = Directory.GetParent(dalDirectory)?.Parent?.Parent?.Parent?.FullName;
        if (projectRoot != null)
        {
            var dalPath = Path.Combine(projectRoot, "Database");
            Directory.CreateDirectory(dalPath); // Ensure directory exists
            return Path.Combine(dalPath, DatabaseName);
        }
        
        // Fallback to local directory if project structure can't be determined
        var localPath = Path.Combine(dalDirectory, "Database");
        Directory.CreateDirectory(localPath);
        return Path.Combine(localPath, DatabaseName);
    }

    private static async Task SeedDataAsync(DbContext context, bool isDevelopment,
        CancellationToken cancellationToken = default)
    {
        if (context is not WeatherDbContext weatherContext) return;

        var seeders = new List<IDataSeed>
        {
            new CityLocationsSeed()
        };

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync(weatherContext, cancellationToken);
        }
    }

    private static void SeedData(DbContext context, bool isDevelopment)
    {
        if (context is not WeatherDbContext weatherContext) return;

        var seeders = new List<IDataSeed>
        {
            new CityLocationsSeed()
        };

        foreach (var seeder in seeders)
        {
            seeder.SeedAsync(weatherContext).GetAwaiter().GetResult();
        }
    }
}