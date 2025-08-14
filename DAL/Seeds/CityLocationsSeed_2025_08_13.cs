using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

internal sealed class LocationMap : ClassMap<Location>
{
    public LocationMap()
    {
        Map(m => m.City).Name("city");
        Map(m => m.Country).Name("country");
        Map(m => m.Latitude).Name("lat").TypeConverterOption.Format("0.#####");
        Map(m => m.Longitude).Name("lng").TypeConverterOption.Format("0.#####");
        Map(m => m.Iso2).Name("iso2");
    }
}

/// <summary>
///     Seed class to populate the Locations table with city data from a CSV file.
///     This is for demo purposes and is designed to be run once through .NET 9's seeder
///     Ideally, use Google Geocoding API or similar for production data.
/// </summary>
public class CityLocationsSeed : IDataSeed
{
    private const int MaxBatchSize = 1000;

    public async Task SeedAsync(WeatherDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Locations.AnyAsync(cancellationToken)) return;

        var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "Seeds", "Data", "worldcities.csv");
        if (!File.Exists(csvPath)) return;

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<LocationMap>();
        var records = csv.GetRecordsAsync<Location>(cancellationToken);
        var locations = new List<Location>();

        await foreach (var location in records)
        {
            locations.Add(location);

            if (locations.Count < MaxBatchSize) continue;

            // Log progress every 1000 records
            Console.WriteLine($"Adding {locations.Count} locations to the database...");
            await context.Locations.AddRangeAsync(locations, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            locations.Clear();
        }

        if (locations.Count > 0)
        {
            await context.Locations.AddRangeAsync(locations, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}