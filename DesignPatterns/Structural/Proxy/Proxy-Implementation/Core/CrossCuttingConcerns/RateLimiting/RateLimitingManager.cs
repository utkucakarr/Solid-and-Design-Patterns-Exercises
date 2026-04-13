using Proxy_Implementation.Models;
using System.Collections.Concurrent;

namespace Proxy_Implementation.Core.CrossCuttingConcerns.RateLimiting
{
    public class RateLimitingManager
    {
        // Thread-safe bir dictionary kullanımı
        private static readonly ConcurrentDictionary<string, int> _requestCounts = new();
        private const int RateLimit = 5;

        public static WeatherResult? CheckRateLimit(string apiKey, string city)
        {
            // Anahtar yoksa 0 değerini alır.
            int count = _requestCounts.GetValueOrDefault(apiKey, 0);
            if (count >= RateLimit)
            {
                Console.WriteLine($"[RateLimit] Limit aşıldı. Key: {apiKey}");
                return WeatherResult.Fail(city, $"Rate limit aşıldı. Dakikada max {RateLimit} istek.");
            }

            _requestCounts[apiKey] = count + 1;
            return null;
        }
    }
}
