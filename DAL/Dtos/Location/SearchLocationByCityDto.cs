using System.ComponentModel.DataAnnotations;

namespace DAL.Dtos.Location;

public class SearchLocationByCityDto : IModelToDto<Models.Location, SearchLocationByCityDto>
{
    [Required]
    [MaxLength(200)]
    [MinLength(2)]
    public string CityName { get; set; }
    
    public static SearchLocationByCityDto MapToDto(Models.Location model)
    {
        // Validate the model before mapping
        // Rely on model field annotations for validation
        if (model == null) throw new ArgumentNullException(nameof(model), "Model cannot be null");

        return new SearchLocationByCityDto()
        {
            CityName = model.City
        };
    }
}