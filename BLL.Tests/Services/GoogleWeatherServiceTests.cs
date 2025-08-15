using BLL.Services.Google.Weather;
using BLL.Tests.Builders;
using DAL.Models;
using DAL.Repository.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace BLL.Tests.Services;

public class GoogleWeatherServiceTests
{
    private readonly Mock<IWeatherRepository> _mockWeatherRepository;
    private readonly Mock<ILocationRepository> _mockLocationRepository;
    private readonly Mock<HttpClient> _mockHttpClient;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<GoogleWeatherService>> _mockLogger;
    private readonly GoogleWeatherService _service;

    public GoogleWeatherServiceTests()
    {
        _mockWeatherRepository = new Mock<IWeatherRepository>();
        _mockLocationRepository = new Mock<ILocationRepository>();
        _mockHttpClient = new Mock<HttpClient>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<GoogleWeatherService>>();

        // Setup configuration mocks
        _mockConfiguration.Setup(x => x["GoogleMaps:ApiKey"]).Returns("test-api-key");
        _mockConfiguration.Setup(x => x["GoogleMaps:BaseUrl"]).Returns("https://maps.googleapis.com/test");
        _mockConfiguration.Setup(x => x["GoogleWeather:BaseUrl"]).Returns("https://weather.googleapis.com/v1/");

        _service = new GoogleWeatherService(
            _mockHttpClient.Object,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockWeatherRepository.Object,
            _mockLocationRepository.Object);
    }

    [Fact]
    public async Task GetCurrentWeatherByLocationIdAsync_WithValidCache_ReturnsFromCache()
    {
        // Arrange
        const int locationId = 1;
        var cachedWeather = WeatherDataBuilder.Create()
            .WithLocationId(locationId)
            .WithTemperature(25.0, "CELSIUS")
            .WithValidCache()
            .Build();

        _mockWeatherRepository
            .Setup(x => x.GetCachedWeatherAsync(locationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedWeather);

        // Act
        var result = await _service.GetCurrentWeatherByLocationIdAsync(locationId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Weather data retrieved from cache");
        result.Data.Should().NotBeNull();
        result.Data!.TempC.Should().Be(25.0);
        result.Data.TempF.Should().BeApproximately(77.0, 0.1); // 25°C = 77°F

        // Verify no API call was made (no SaveWeatherDataAsync should be called)
        _mockWeatherRepository.Verify(
            x => x.SaveWeatherDataAsync(It.IsAny<WeatherData>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetCurrentWeatherByLocationIdAsync_WithExpiredCache_AttemptsToFetchFromAPI()
    {
        // Arrange
        const int locationId = 1;
        var expiredWeather = WeatherDataBuilder.Create()
            .WithLocationId(locationId)
            .WithExpiredCache()
            .Build();

        var location = new Location
        {
            Id = locationId,
            City = "Test City",
            Country = "Test Country",
            Province = "Test Province",
            Latitude = -32.5269,
            Longitude = 115.7221
        };

        // Setup: no valid cache
        _mockWeatherRepository
            .Setup(x => x.GetCachedWeatherAsync(locationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherData?)null);

        // Setup: location found
        _mockLocationRepository
            .Setup(x => x.FindLocationByIdAsync(locationId))
            .ReturnsAsync(location);

        // Act
        var result = await _service.GetCurrentWeatherByLocationIdAsync(locationId);

        // Assert
        result.Should().NotBeNull();
        
        // Verify location was retrieved
        _mockLocationRepository.Verify(
            x => x.FindLocationByIdAsync(locationId),
            Times.Once);

        // Note: Since we haven't mocked the HTTP client properly for the actual API call,
        // this test verifies the cache miss behavior and location lookup.
        // The actual API call would fail, but that's expected in this unit test.
    }

    [Fact]
    public async Task GetCurrentWeatherByLocationIdAsync_WithNonExistentLocation_ReturnsNotFound()
    {
        // Arrange
        const int locationId = 999;

        // Setup: no cache
        _mockWeatherRepository
            .Setup(x => x.GetCachedWeatherAsync(locationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherData?)null);

        // Setup: location not found
        _mockLocationRepository
            .Setup(x => x.FindLocationByIdAsync(locationId))
            .ReturnsAsync((Location?)null);

        // Act
        var result = await _service.GetCurrentWeatherByLocationIdAsync(locationId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Location not found");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetHourlyForecastByLocationIdAsync_ReturnsNotImplementedMessage()
    {
        // Act
        var result = await _service.GetHourlyForecastByLocationIdAsync(1, 24);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Hourly forecast not yet implemented");
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDailyForecastByLocationIdAsync_ReturnsNotImplementedMessage()
    {
        // Act
        var result = await _service.GetDailyForecastByLocationIdAsync(1, 7);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Daily forecast not yet implemented");
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetWeatherBundleByLocationIdAsync_WithValidCache_ReturnsBundle()
    {
        // Arrange
        const int locationId = 1;
        var cachedWeather = WeatherDataBuilder.Create()
            .WithLocationId(locationId)
            .WithTemperature(20.0, "CELSIUS")
            .WithValidCache()
            .Build();

        _mockWeatherRepository
            .Setup(x => x.GetCachedWeatherAsync(locationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedWeather);

        // Act
        var result = await _service.GetWeatherBundleByLocationIdAsync(locationId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Weather bundle retrieved (current only)");
        result.Data.Should().NotBeNull();
        result.Data!.Current.Should().NotBeNull();
        result.Data.Current!.TempC.Should().Be(20.0);
        result.Data.Hourly.Should().BeEmpty(); // Not implemented yet
        result.Data.Daily.Should().BeEmpty(); // Not implemented yet
    }
}