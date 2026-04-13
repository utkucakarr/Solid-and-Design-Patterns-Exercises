using FluentAssertions;
using Moq;
using Proxy_Implementation.Interfaces;
using Proxy_Implementation.Models;
using Proxy_Implementation.Proxy;

namespace Proxy_Tests
{
    public class WeatherServiceProxyTests
    {
        private readonly Mock<IWeatherService> _realServiceMock;
        private const string InvalidKey = "INVALID_KEY";

        // Her test kendi unique key'ini üretiyor - static dictionary kirlenmez
        private static string UniqueKey => $"VALID_KEY";

        public WeatherServiceProxyTests()
        {
            _realServiceMock = new Mock<IWeatherService>();

            _realServiceMock
                .Setup(s => s.GetWeather(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string city, string key) => WeatherResult.Success(city, "22°C, Güneşli"));

            _realServiceMock
                .Setup(s => s.GetForecast(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns((string city, int days, string key) =>
                    WeatherResult.Success(city, $"{days} günlük tahmin hazır."));
        }

        private WeatherServiceProxy CreateSut()
            => new WeatherServiceProxy(_realServiceMock.Object);

        // --- GetWeather - Başarılı Senaryo ---

        [Fact]
        public void GetWeather_ShouldReturnCorrectCity()
        {
            var sut = CreateSut();
            var result = sut.GetWeather("İstanbul", UniqueKey);

            result.City.Should().Be("İstanbul");
        }

        // --- Authentication Testleri ---

        [Fact]
        public void GetWeather_WithInvalidApiKey_ShouldReturnFail()
        {
            var sut = CreateSut();
            var result = sut.GetWeather("İstanbul", InvalidKey);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void GetWeather_WithInvalidApiKey_ShouldNotCallRealService()
        {
            var sut = CreateSut();
            sut.GetWeather("İstanbul", InvalidKey);

            _realServiceMock.Verify(
                s => s.GetWeather(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void GetForecast_WithInvalidApiKey_ShouldReturnFail()
        {
            var sut = CreateSut();
            var result = sut.GetForecast("İstanbul", 3, InvalidKey);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void GetForecast_WithInvalidApiKey_ShouldNotCallRealService()
        {
            var sut = CreateSut();
            sut.GetForecast("İstanbul", 3, InvalidKey);

            _realServiceMock.Verify(
                s => s.GetForecast(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()),
                Times.Never);
        }

        // --- Rate Limit Testleri ---

        [Fact]
        public void GetWeather_WhenRateLimitExceeded_ShouldReturnFail()
        {
            var sut = CreateSut();
            var apiKey = UniqueKey;

            for (int i = 0; i < 5; i++)
                sut.GetWeather("İstanbul", apiKey);

            var result = sut.GetWeather("İstanbul", apiKey);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void GetWeather_WhenRateLimitExceeded_ShouldNotCallRealService()
        {
            var sut = CreateSut();
            var apiKey = UniqueKey;

            for (int i = 0; i < 5; i++)
                sut.GetWeather("İstanbul", apiKey);

            sut.GetWeather("İstanbul", apiKey);

            // Gerçek servis yalnızca 5 kez çağrılmış olmalı
            _realServiceMock.Verify(
                s => s.GetWeather(It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(5));
        }


        // --- Guard Clause Testleri ---

        [Fact]
        public void GetWeather_WithEmptyCity_ShouldThrowArgumentException()
        {
            var sut = CreateSut();
            var act = () => sut.GetWeather(" ", UniqueKey);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetWeather_WithEmptyApiKey_ShouldThrowArgumentException()
        {
            var sut = CreateSut();
            var act = () => sut.GetWeather("İstanbul", " ");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetForecast_WithZeroDays_ShouldThrowArgumentOutOfRangeException()
        {
            var sut = CreateSut();
            var act = () => sut.GetForecast("İstanbul", 0, UniqueKey);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void GetForecast_WithNegativeDays_ShouldThrowArgumentOutOfRangeException()
        {
            var sut = CreateSut();
            var act = () => sut.GetForecast("İstanbul", -1, UniqueKey);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        // --- Constructor Testleri ---

        [Fact]
        public void Constructor_WithNullRealService_ShouldThrowArgumentNullException()
        {
            var act = () => new WeatherServiceProxy(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("realService");
        }

        // --- Proxy Garantisi ---

        [Fact]
        public void WeatherServiceProxy_ShouldImplement_IWeatherService()
        {
            var sut = CreateSut();
            sut.Should().BeAssignableTo<IWeatherService>();
        }
    }
}