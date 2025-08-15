using BLL.Services.Weather;
using FluentAssertions;

namespace BLL.Tests.Services;

public class WeatherUnitConverterTests
{
    [Theory]
    [InlineData(0, 32)]
    [InlineData(25, 77)]
    [InlineData(100, 212)]
    [InlineData(-40, -40)]
    public void CelsiusToFahrenheit_WithValidTemperatures_ReturnsCorrectConversion(double celsius, double expectedFahrenheit)
    {
        // Act
        var result = WeatherUnitConverter.CelsiusToFahrenheit(celsius);

        // Assert
        result.Should().BeApproximately(expectedFahrenheit, 0.01);
    }

    [Theory]
    [InlineData(32, 0)]
    [InlineData(77, 25)]
    [InlineData(212, 100)]
    [InlineData(-40, -40)]
    public void FahrenheitToCelsius_WithValidTemperatures_ReturnsCorrectConversion(double fahrenheit, double expectedCelsius)
    {
        // Act
        var result = WeatherUnitConverter.FahrenheitToCelsius(fahrenheit);

        // Assert
        result.Should().BeApproximately(expectedCelsius, 0.01);
    }

    [Theory]
    [InlineData(100, 62.14)] // 100 km/h ≈ 62.14 mph
    [InlineData(60, 37.28)]  // 60 km/h ≈ 37.28 mph
    [InlineData(0, 0)]       // 0 km/h = 0 mph
    public void KilometersPerHourToMilesPerHour_WithValidSpeeds_ReturnsCorrectConversion(double kph, double expectedMph)
    {
        // Act
        var result = WeatherUnitConverter.KilometersPerHourToMilesPerHour(kph);

        // Assert
        result.Should().BeApproximately(expectedMph, 0.01);
    }

    [Theory]
    [InlineData(62.14, 100)] // 62.14 mph ≈ 100 km/h
    [InlineData(37.28, 60)]  // 37.28 mph ≈ 60 km/h
    [InlineData(0, 0)]       // 0 mph = 0 km/h
    public void MilesPerHourToKilometersPerHour_WithValidSpeeds_ReturnsCorrectConversion(double mph, double expectedKph)
    {
        // Act
        var result = WeatherUnitConverter.MilesPerHourToKilometersPerHour(mph);

        // Assert
        result.Should().BeApproximately(expectedKph, 0.01);
    }

    [Theory]
    [InlineData(10, 36)]     // 10 m/s = 36 km/h
    [InlineData(5, 18)]      // 5 m/s = 18 km/h
    [InlineData(0, 0)]       // 0 m/s = 0 km/h
    public void MetersPerSecondToKilometersPerHour_WithValidSpeeds_ReturnsCorrectConversion(double mps, double expectedKph)
    {
        // Act
        var result = WeatherUnitConverter.MetersPerSecondToKilometersPerHour(mps);

        // Assert
        result.Should().BeApproximately(expectedKph, 0.01);
    }

    [Theory]
    [InlineData(0, "N")]
    [InlineData(45, "NE")]
    [InlineData(90, "E")]
    [InlineData(135, "SE")]
    [InlineData(180, "S")]
    [InlineData(225, "SW")]
    [InlineData(270, "W")]
    [InlineData(315, "NW")]
    [InlineData(360, "N")]
    [InlineData(361, "N")] // Should wrap around
    public void DegreesToCardinalDirection_WithValidDegrees_ReturnsCorrectDirection(double degrees, string expectedDirection)
    {
        // Act
        var result = WeatherUnitConverter.DegreesToCardinalDirection(degrees);

        // Assert
        result.Should().Be(expectedDirection);
    }

    [Theory]
    [InlineData(-45, "NW")] // Negative degrees should work
    [InlineData(-90, "W")]
    public void DegreesToCardinalDirection_WithNegativeDegrees_ReturnsCorrectDirection(double degrees, string expectedDirection)
    {
        // Act
        var result = WeatherUnitConverter.DegreesToCardinalDirection(degrees);

        // Assert
        result.Should().Be(expectedDirection);
    }

    [Fact]
    public void NormalizeTemperature_WithCelsius_ReturnsCorrectBothUnits()
    {
        // Arrange
        const double temperatureCelsius = 25.0;
        const string unit = "CELSIUS";

        // Act
        var (tempC, tempF) = WeatherUnitConverter.NormalizeTemperature(temperatureCelsius, unit);

        // Assert
        tempC.Should().Be(25.0);
        tempF.Should().BeApproximately(77.0, 0.01);
    }

    [Fact]
    public void NormalizeTemperature_WithFahrenheit_ReturnsCorrectBothUnits()
    {
        // Arrange
        const double temperatureFahrenheit = 77.0;
        const string unit = "FAHRENHEIT";

        // Act
        var (tempC, tempF) = WeatherUnitConverter.NormalizeTemperature(temperatureFahrenheit, unit);

        // Assert
        tempC.Should().BeApproximately(25.0, 0.01);
        tempF.Should().Be(77.0);
    }

    [Fact]
    public void NormalizeTemperature_WithNullTemperature_ReturnsDefaultValues()
    {
        // Act
        var (tempC, tempF) = WeatherUnitConverter.NormalizeTemperature(null, "CELSIUS");

        // Assert
        tempC.Should().Be(0);
        tempF.Should().Be(32);
    }

    [Fact]
    public void NormalizeWindSpeed_WithKilometersPerHour_ReturnsCorrectBothUnits()
    {
        // Arrange
        const double windSpeedKph = 100.0;
        const string unit = "KILOMETERS_PER_HOUR";

        // Act
        var (kph, mph) = WeatherUnitConverter.NormalizeWindSpeed(windSpeedKph, unit);

        // Assert
        kph.Should().Be(100.0);
        mph.Should().BeApproximately(62.14, 0.01);
    }

    [Fact]
    public void NormalizeWindSpeed_WithMetersPerSecond_ReturnsCorrectBothUnits()
    {
        // Arrange
        const double windSpeedMps = 10.0;
        const string unit = "METERS_PER_SECOND";

        // Act
        var (kph, mph) = WeatherUnitConverter.NormalizeWindSpeed(windSpeedMps, unit);

        // Assert
        kph.Should().BeApproximately(36.0, 0.01); // 10 m/s = 36 km/h
        mph.Should().BeApproximately(22.37, 0.01); // 10 m/s ≈ 22.37 mph
    }

    [Fact]
    public void NormalizeWindSpeed_WithNullSpeed_ReturnsNullValues()
    {
        // Act
        var (kph, mph) = WeatherUnitConverter.NormalizeWindSpeed(null, "KPH");

        // Assert
        kph.Should().BeNull();
        mph.Should().BeNull();
    }
}