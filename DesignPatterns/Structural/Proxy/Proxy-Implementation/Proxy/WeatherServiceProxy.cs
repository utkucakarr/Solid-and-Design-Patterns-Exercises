using Proxy_Implementation.Core.CrossCuttingConcerns.Logging;
using Proxy_Implementation.Core.CrossCuttingConcerns.RateLimiting;
using Proxy_Implementation.Core.CrossCuttingConcerns.Security;
using Proxy_Implementation.Interfaces;
using Proxy_Implementation.Models;

namespace Proxy_Implementation.Proxy
{
    public class WeatherServiceProxy : IWeatherService
    {
        // Protection Proxy - gerçek servisle aynı interface'i implemet ediyor
        // Client proxy mi gerçek servis mi olduğunu bilmiyor
        // Sorumluluklar: API key doğrulama, rate limiting, loglama

        private readonly IWeatherService _realService;

        public WeatherServiceProxy(IWeatherService realService)
        {
            _realService = realService
                ?? throw new ArgumentNullException(nameof(realService));
        }

        // --- MERKEZİ İŞLEYİCİ (DRY Prensibi) ---
        private WeatherResult ExecuteCrossCuttingConcerns(
            string apiKey, string city, string methodName, Func<WeatherResult> targetMethod)
        {
            // 1. API key doğrulama
            var authResult = AuthManager.Authenticate(apiKey);
            if (authResult is not null) return authResult;

            // 2. Rate limit kontrolü
            var rateLimitResult = RateLimitingManager.CheckRateLimit(apiKey, city);
            if (rateLimitResult is not null) return rateLimitResult;

            // 3. Loglama
            Logger.LogInfo(apiKey, methodName, city);

            // 4. Gerçek servise ilet
            return targetMethod();
        }

        // --- INTERFACE UYGULAMALARI ---
        public WeatherResult GetForecast(string city, int days, string apiKey)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(days, nameof(days));
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey, nameof(apiKey));

            return ExecuteCrossCuttingConcerns(apiKey, city, nameof(GetForecast), () =>
            {
                return _realService.GetForecast(city, days, apiKey);
            });

        }

        public WeatherResult GetWeather(string city, string apiKey)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey, nameof(apiKey));

            return ExecuteCrossCuttingConcerns(apiKey, city, nameof(GetForecast), () =>
            {
                return _realService.GetWeather(city, apiKey);
            });
        }
    }
}
