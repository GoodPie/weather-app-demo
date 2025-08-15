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
    public string Label { get; set; }


    public static SearchLocationResponseDto MapToDto(Models.Location model)
    {
        // Validate the model before mapping
        // Rely on model field annotations for validation
        if (model == null) throw new ArgumentNullException(nameof(model), "Model cannot be null");

        // Not a big fan of this but look at later
        var label = new List<string> { model.City, model.Province, model.Country }
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Aggregate((current, next) => $"{current}, {next}");

        return new SearchLocationResponseDto
        {
            Id = model.Id,
            Label = label
        };
    }
}