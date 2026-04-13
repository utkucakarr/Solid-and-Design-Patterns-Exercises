using Proxy_Implementation.Models;

namespace Proxy_Implementation.Interfaces
{
    // Hem gerçek servis hem proxy bu interface'i implement ediyor
    // Client proxy mi gerçek servis mi olduğunu bilmiyor.
    public interface IWeatherService
    {
        WeatherResult GetWeather(string city, string apiKey);
        WeatherResult GetForecast(string city, int days, string apiKey);
    }
}
