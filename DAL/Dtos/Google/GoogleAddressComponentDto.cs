namespace DAL.Dtos.Google;

public class GoogleAddressComponentDto
{
    public string LongName { get; set; } = "";
    public string ShortName { get; set; } = "";

    public List<string> Types { get; set; } = new();
}