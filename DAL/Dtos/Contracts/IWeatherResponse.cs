using DAL.Dtos.Location;

namespace DAL.Dtos.Contracts;

public interface IWeatherResponse<T>
{
    public T? Weather { get; set; }
    public SearchLocationResponseDto? Location { get; set; }
}