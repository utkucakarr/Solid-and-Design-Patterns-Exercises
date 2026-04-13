using Proxy_Implementation.Interfaces;
using Proxy_Implementation.Models;

namespace Proxy_Implementation.Services
{
    // RealSubject - sadece iş mantığını biliyor
    // Auth, rate limit, loglama bilmiyor - bunlar Proxy'nin sorumluluğu
    public class WeatherService : IWeatherService
    {
        public WeatherResult GetForecast(string city, int days, string apiKey)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(days, nameof(days));
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey, nameof(apiKey));

            Console.WriteLine($"[Gerçek Servis] {city} için {days} günlük tahmin getiriliyor.");
            return WeatherResult.Success(city, $"{days} günlük tahmin: 22°C, 23°C, 21°C...");
        }

        public WeatherResult GetWeather(string city, string apiKey)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey, nameof(apiKey));

            Console.WriteLine($"[Gerçek Servis] {city} hava durumu getiriliyor.");
            return WeatherResult.Success(city, "22°C, Güneşli");
        }
    }
}
