namespace Proxy_Violation.Weather
{
    // Gerçek servis - dış API'yı çağırıyor
    public class RealWeatherServiceBad
    {
        public string GetWeather(string city)
        {
            Console.WriteLine($"[Gerçek Servis] {city} hava durumu getiriliyor.");
            return $"{city}: 22°C, Güneşli";
        }

        public string GetForecast(string city, int days)
        {
            Console.WriteLine($"[Gerçek Servis] {city} için {days} günlük tahmin getiriliyor.");
            return $"{city}: {days} günlük tahmin hazır.";
        }
    }
}
