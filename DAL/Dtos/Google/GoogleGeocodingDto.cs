namespace DAL.Dtos.Google;

public class GoogleGeocodingDto
{
    public string Status { get; set; } = "";
    public List<GoogleGeocodingResultDto> Results { get; set; } = new();
}