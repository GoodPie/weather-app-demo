using DAL.Dtos.Contracts;

namespace DAL.Dtos.Location;

public class GeocodingLocationDto : IDtoToModel<GeocodingLocationDto, Models.Location>
{
    public string City { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }

    public string ProvinceName { get; set; }
    public string Country { get; set; }
    public string? Code { get; set; }


    public static Models.Location MapToModel(GeocodingLocationDto dto)
    {
        return new Models.Location
        {
            City = dto.City,
            Latitude = double.Parse(dto.Latitude),
            Longitude = double.Parse(dto.Longitude),
            Province = dto.ProvinceName,
            Country = dto.Country,
            Iso2 = dto.Code
        };
    }
}