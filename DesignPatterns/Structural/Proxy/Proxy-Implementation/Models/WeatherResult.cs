namespace Proxy_Implementation.Models
{
    public class WeatherResult
    {
        public bool IsSuccess { get; }
        public string City { get; }
        public string Data { get; }
        public string Message { get; }

        private WeatherResult(
            bool isSuccess,
            string city,
            string data,
            string message)
        {
            IsSuccess = isSuccess;
            City = city;
            Data = data;
            Message = message;
        }

        public static WeatherResult Success(string city, string data)
            => new(true, city, data, $"[{city}] {data}");

        public static WeatherResult Fail(string city, string reason)
            => new(false, city, string.Empty, $"[{city}] Hata: {reason}");
    }
}
