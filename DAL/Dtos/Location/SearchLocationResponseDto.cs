using DAL.Dtos.Contracts;

namespace DAL.Dtos.Location;

/// <summary>
///     This is the generic location response DTO used for searching locations
///     Provides basic information about a location such as city name, country, latitude, and longitude to
///     be used in search results.
/// </summary>
public class SearchLocationResponseDto : IModelToDto<Models.Location, SearchLocationResponseDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public string Label { get; set; }
    public string Code { get; set; } = string.Empty;

    public static SearchLocationResponseDto MapToDto(Models.Location model)
    {
        // Validate the model before mapping
        // Rely on model field annotations for validation
        if (model == null) throw new ArgumentNullException(nameof(model), "Model cannot be null");

        return new SearchLocationResponseDto
        {
            Id = model.Id,
            Name = model.City,
            Country = model.Country,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            Label = $"{model.City}, {model.Country}",
            Code = model.Iso2 ?? string.Empty
        };
    }
}