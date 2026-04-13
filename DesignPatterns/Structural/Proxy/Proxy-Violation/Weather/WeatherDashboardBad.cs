namespace Proxy_Violation.Weather
{
    // Her client kendi kontrollerini kendisi yapıyor.
    public class WeatherDashboardBad
    {
        private readonly RealWeatherServiceBad _service = new();
        private readonly Dictionary<string, int> _requestCount = new();

        // Api key kontrolü her client'ta tekrarlanıyor!
        public void ShowWeather(string city, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey != "VALID_KEY")
            {
                Console.WriteLine("Geçersiz API key!");
                return;
            }

            // Rate limit kontrolü burada - başka bir client'ta tekrar yazılacak
            _requestCount.TryGetValue(apiKey, out var count);
            if(count >= 5)
            {
                Console.WriteLine("Rate limit aşıldı! Dakikada max 5 istek.");
                return;
            }

            _requestCount[apiKey] = count + 1;

            // Loglama burada - başka bir client'ta tekrar yazılacak
            Console.WriteLine($"[Log] API Key: {apiKey} | İstek #{count + 1}");

            var result = _service.GetWeather(city);
            Console.WriteLine(result);
        }
    }
}
