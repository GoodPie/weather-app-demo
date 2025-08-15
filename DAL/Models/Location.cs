using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index(nameof(City))]
[Index(nameof(Latitude), nameof(Longitude))]
[Index(nameof(Country))]
public class Location : TimeStampedEntity
{
    // Unique identifier for the location
    public int Id { get; set; }

    // Basic searchable data
    [Required] [MaxLength(200)] public required string City { get; set; }

    [Required] [MaxLength(200)] public required string Province { get; set; }

    [Required] [MaxLength(100)] public required string Country { get; set; }

    // Geolocation data
    [Precision(10, 5)] public double Latitude { get; set; }

    [Precision(10, 5)] public double Longitude { get; set; }

    // ISO codes for the country (nullable since they might not always be available)
    [MaxLength(2)] public string? Iso2 { get; set; }
}