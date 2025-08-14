namespace BLL.Services.Contracts;

public interface IGeocodingService
{
    Task<(double lat, double lng)> GetLatLngByCityAsync(string city, CancellationToken ct = default);
}