namespace DAL.Dtos.Google;

public class GoogleGeocodingResultDto
{
    public GoogleGeocodingGeometry Geometry { get; set; } = new();
    public string FormattedAddress { get; set; } = "";
    public List<GoogleAddressComponentDto> AddressComponents { get; set; } = new();
}