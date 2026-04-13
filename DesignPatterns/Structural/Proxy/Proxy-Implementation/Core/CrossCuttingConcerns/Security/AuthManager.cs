using Proxy_Implementation.Models;

namespace Proxy_Implementation.Core.CrossCuttingConcerns.Security
{
    public class AuthManager
    {
        private const string ValidApiKey = "VALID_KEY";
        public static WeatherResult? Authenticate(string apiKey)
        {
            if(apiKey != ValidApiKey)
            {
                Console.WriteLine($"[Security] Geçersiz API key: {apiKey}");
                return WeatherResult.Fail("N/A", "Geçersiz API key.");
            }
            return null;
        }
    }
}
