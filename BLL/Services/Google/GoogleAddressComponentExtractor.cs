using DAL.Dtos.Google;
using DAL.Dtos.Location;

namespace BLL.Services.Google;

public class GoogleAddressComponentExtractor(GoogleGeocodingResultDto result)
{
    private const string CountryType = "country";
    private const string LocalityType = "locality";
    private const string AdminLevel1Type = "administrative_area_level_1";
    private const string AdminLevel2Type = "administrative_area_level_2";

    public GeocodingLocationDto ExtractLocationData()
    {
        var country = ExtractCountry();
        if (string.IsNullOrEmpty(country))
            throw new InvalidOperationException("Country information is missing in the geocoding result.");

        return new GeocodingLocationDto
        {
            City = ExtractCityName(),
            Latitude = ExtractLatitude(),
            Longitude = ExtractLongitude(),
            ProvinceName = ExtractProvinceName(),
            Country = country,
            Code = ExtractIso2()
        };
    }

    private string ExtractCountry()
    {
        var countryComponent = FindAddressComponent(CountryType);
        return countryComponent?.LongName ?? string.Empty;
    }

    private string ExtractIso2()
    {
        var countryComponent = FindAddressComponent(CountryType);
        return countryComponent?.ShortName ?? string.Empty;
    }

    private string ExtractCityName()
    {
        var localityComponent = FindAddressComponent(LocalityType);

        // If locality is not found, try to use administrative_area_level_1 as a fallback
        if (localityComponent != null) return localityComponent.LongName ?? string.Empty;

        var adminLevel1Component = FindAddressComponent(AdminLevel1Type);
        return adminLevel1Component != null ? adminLevel1Component.LongName : string.Empty;
    }

    private string ExtractProvinceName()
    {
        var adminLevel1Component = FindAddressComponent(AdminLevel1Type);
        return adminLevel1Component?.ShortName ?? string.Empty;
    }

    private string ExtractLatitude()
    {
        return result.Geometry.Location.Lat.ToString();
    }

    private string ExtractLongitude()
    {
        return result.Geometry.Location.Lng.ToString();
    }

    private GoogleAddressComponentDto? FindAddressComponent(string type)
    {
        return result.AddressComponents.FirstOrDefault(ac => ac.Types.Contains(type));
    }
}