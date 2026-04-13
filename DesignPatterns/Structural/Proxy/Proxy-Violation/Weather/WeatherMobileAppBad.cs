namespace Proxy_Violation.Weather
{
    public class WeatherMobileAppBad
    {
        private readonly RealWeatherServiceBad _service = new();
        private readonly Dictionary<string, int> _requestCounts = new();

        // Aynı kontroller burada da tekrarlanıyor — DRY ihlali!
        public void DisplayWeather(string city, string apiKey)
        {
            if(string.IsNullOrWhiteSpace(city) || apiKey != "VALID_KEY")
            {
                Console.WriteLine("Geçersiz API key!");
                return;
            }

            _requestCounts.TryGetValue(apiKey, out var count);

            if (count >= 5)
            {
                Console.WriteLine("Rate limit aşıldı!");
                return;
            }

            _requestCounts[apiKey] = count + 1;
            Console.WriteLine($"[Log] API Key: {apiKey} | İstek #{count + 1}");

            var result = _service.GetWeather(city);
            Console.WriteLine(result);

            // Yeni kontrol (IP block, quota) = her client'a dokun!
        }
    }
}
