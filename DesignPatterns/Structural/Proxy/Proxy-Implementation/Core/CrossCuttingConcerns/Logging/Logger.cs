namespace Proxy_Implementation.Core.CrossCuttingConcerns.Logging
{
    public class Logger
    {
        public static void LogInfo(string apiKey, string operation, string city)
        {
            Console.WriteLine($"[Log] Key: {apiKey} | Op: {operation} | City: {city}");
        }
    }
}
