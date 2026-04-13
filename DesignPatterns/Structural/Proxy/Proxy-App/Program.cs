using Proxy_Implementation.Interfaces;
using Proxy_Implementation.Proxy;
using Proxy_Implementation.Services;
using Proxy_Violation.Weather;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   PROXY İHLALİ — CANLI DEMO           ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var dashboard = new WeatherDashboardBad();
var mobileApp = new WeatherMobileAppBad();

Console.WriteLine("--- Dashboard İstekleri (Auth + Rate Limit client'ta) ---\n");
dashboard.ShowWeather("İstanbul", "VALID_KEY");
dashboard.ShowWeather("Ankara", "INVALID_KEY");

Console.WriteLine();
Console.WriteLine("--- Mobile App İstekleri (Aynı kontroller tekrar yazıldı) ---\n");
mobileApp.DisplayWeather("İzmir", "VALID_KEY");

Console.WriteLine();
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> Auth ve rate limit kontrolü her client'ta tekrarlanıyor");
Console.WriteLine("  -> Yeni kontrol (IP block) = tüm client'lara dokun");
Console.WriteLine("  -> Test edilemez — kontrol ve iş mantığı iç içe geçmiş\n");

Console.WriteLine("╔══════════════════════════════════════════════════╗");
Console.WriteLine("║  PROXY & CROSS-CUTTING CONCERNS — DOĞRU YAKLAŞIM ║");
Console.WriteLine("╚══════════════════════════════════════════════════╝\n");

// Client sadece IWeatherService biliyor. 
// Arka planda loglama veya rate limit mimarisinin değiştiğinden haberi bile yok!
IWeatherService service = new WeatherServiceProxy(new WeatherService());

Console.WriteLine("--- Geçerli API Key ile İstekler ---\n");
var result1 = service.GetWeather("İstanbul", "VALID_KEY");
Console.WriteLine(result1.IsSuccess ? $" Success {result1.Message}" : $" Fail: {result1.Message}");

var result2 = service.GetForecast("Ankara", 3, "VALID_KEY");
Console.WriteLine(result2.IsSuccess ? $" Success: {result2.Message}" : $" Fail: {result2.Message}");

Console.WriteLine();
Console.WriteLine("--- Geçersiz API Key ile İstek ---\n");
var result3 = service.GetWeather("İzmir", "INVALID_KEY");
Console.WriteLine(result3.IsSuccess ? $" Success: {result3.Message}" : $" Fail: {result3.Message}");

Console.WriteLine();
Console.WriteLine("--- Rate Limit Testi (5 istek sonrası) ---\n");
for (int i = 1; i <= 6; i++)
{
    var result = service.GetWeather("Bursa", "VALID_KEY");
    Console.WriteLine(result.IsSuccess
        ? $"  Success İstek #{i}: {result.Message}"
        : $"  Fail İstek #{i}: {result.Message}");
}

Console.WriteLine();
Console.WriteLine("  IP block veya Cache (Önbellek) eklemek istesek:");
Console.WriteLine("  -> Core/CrossCuttingConcerns altına yeni bir Manager sınıfı açılacak.");
Console.WriteLine("  -> Proxy'deki ExecuteWithCrossCuttingConcerns metoduna 1 satır eklenecek.");
Console.WriteLine("  -> Client (İstemci) KESİNLİKLE DEĞİŞMEYECEK!\n");

Console.WriteLine("--- SONUÇ ---");
Console.WriteLine("  Bad -> Auth + rate limit her metotta veya her client'ta kopyala-yapıştır yapılıyor.");
Console.WriteLine("  Good -> Proxy deseni ile araya girip merkezi kontrol sağlanıyor.");
Console.WriteLine("  Best -> Proxy + Core Katmanı birleşimi ile Sorumlulukların Tekliği (SRP) ve DRY prensiplerine tam uyum sağlanıyor!\n");