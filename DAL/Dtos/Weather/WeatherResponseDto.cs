using DAL.Dtos.Contracts;
using DAL.Dtos.Location;

namespace DAL.Dtos.Weather;

public class WeatherResponseDto<T> : IWeatherResponse<T>
{
    public T? Weather { get; set; }
    public SearchLocationResponseDto? Location { get; set; }
}