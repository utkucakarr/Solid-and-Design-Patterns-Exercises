# Design Patterns #9 — Proxy Pattern

> *"Başka bir nesneye erişimi kontrol etmek için o nesnenin yerini tutan veya ona bir aracı görevi gören nesne."*
> — Gang of Four (GoF)

---

## Pattern Nedir?

Proxy Pattern, bir nesneye erişimi kontrol etmek amacıyla o nesneyle aynı interface'i implement eden ve araya giren yapısal bir tasarım desenidir. Client, gerçek nesneyle mi yoksa proxy ile mi çalıştığını bilmez — her ikisi de aynı interface'i sunar.

**Ne zaman kullanılır?**

- Gerçek nesneye erişim öncesinde kontrol gerekiyorsa (auth, rate limit)
- Cross-cutting concern'leri iş mantığından ayırmak gerekiyorsa
- Gerçek nesne pahalıysa ve lazy initialization gerekiyorsa
- Her isteği loglamak veya izlemek gerekiyorsa

**Proxy Türleri:**
- **Protection Proxy** — Erişim kontrolü (auth, rate limit, IP block)
- **Virtual Proxy** — Pahalı nesneyi gerektiğinde oluştur (lazy init)
- **Logging Proxy** — Her işlemi logla ve izle
- **Remote Proxy** — Uzaktaki nesneyi yerel gibi kullan

**Gerçek hayat örnekleri:**
- API Gateway — auth, rate limiting, loglama
- ORM Lazy Loading — ilişkili veri gerçekten kullanılana kadar yüklenmiyor
- CDN — gerçek sunucu yerine edge server önce yanıt veriyor
- Spring AOP / .NET Interceptors — cross-cutting concerns

---

## Kötü Kullanım — Proxy İhlali

Her client aynı auth ve rate limit kontrollerini kendisi yazıyor:

```csharp
public class WeatherDashboard_Bad
{
    public void ShowWeather(string city, string apiKey)
    {
        // Auth kontrolü her client'ta — DRY ihlali!
        if (apiKey != "VALID_KEY") { return; }

        // Rate limit her client'ta — DRY ihlali!
        if (count >= 5) { return; }

        // Loglama her client'ta — DRY ihlali!
        Console.WriteLine($"[Log] Key: {apiKey}");

        _service.GetWeather(city);
    }
}

public class WeatherMobileApp_Bad
{
    public void DisplayWeather(string city, string apiKey)
    {
        // Aynı 3 kontrol burada da tekrar yazıldı!
        if (apiKey != "VALID_KEY") { return; }
        if (count >= 5) { return; }
        Console.WriteLine($"[Log] Key: {apiKey}");

        _service.GetWeather(city);
        // Yeni kontrol (IP block) = her client'a dokun!
    }
}
```

### Sonuçlar:

| Sorun | Açıklama |
|---|---|
| DRY ihlali | Auth + rate limit her client'ta tekrar yazılıyor |
| OCP ihlali | Yeni kontrol = tüm client'lara dokun |
| Thread-safety yok | Her client kendi sayacını tutuyor — race condition riski |
| Test zorluğu | Kontrol ve iş mantığı iç içe — izole test edilemiyor |

---

## ✅ Doğru Kullanım — Proxy Pattern

### Cross-Cutting Concerns Katmanı

Her sorumluluk kendi sınıfına ayrıldı:

```csharp
// Merkezi loglama
public class Logger
{
    public static void LogInfo(string apiKey, string operation, string city)
        => Console.WriteLine($"[Log] Key: {apiKey} | Op: {operation} | City: {city}");
}

// Thread-safe rate limiting — ConcurrentDictionary ile race condition önleniyor
public class RateLimitingManager
{
    private static readonly ConcurrentDictionary<string, int> _requestCounts = new();
    private const int RateLimit = 5;

    public static WeatherResult? CheckRateLimit(string apiKey, string city)
    {
        int count = _requestCounts.GetValueOrDefault(apiKey, 0);
        if (count >= RateLimit)
            return WeatherResult.Fail(city, $"Rate limit aşıldı. Max {RateLimit} istek.");

        _requestCounts[apiKey] = count + 1;
        return null;
    }
}

// Merkezi auth
public class AuthManager
{
    private const string ValidApiKey = "VALID_KEY";

    public static WeatherResult? Authenticate(string apiKey)
    {
        if (apiKey != ValidApiKey)
            return WeatherResult.Fail("N/A", "Geçersiz API key.");
        return null;
    }
}
```

### Proxy — DRY Pipeline

Tüm cross-cutting concern'ler merkezi bir pipeline'da toplanıyor:

```csharp
public class WeatherServiceProxy : IWeatherService
{
    private readonly IWeatherService _realService;

    // Merkezi pipeline — DRY prensibi korunuyor
    private WeatherResult ExecuteCrossCuttingConcerns(
        string apiKey, string city, string methodName, Func<WeatherResult> targetMethod)
    {
        var authResult = AuthManager.Authenticate(apiKey);       // 1. Auth
        if (authResult is not null) return authResult;

        var rateResult = RateLimitingManager.CheckRateLimit(apiKey, city); // 2. Rate limit
        if (rateResult is not null) return rateResult;

        Logger.LogInfo(apiKey, methodName, city);               // 3. Log

        return targetMethod();                                   // 4. Gerçeğe ilet
    }

    public WeatherResult GetWeather(string city, string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(city,   nameof(city));
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey, nameof(apiKey));

        return ExecuteCrossCuttingConcerns(apiKey, city, nameof(GetWeather), ()
            => _realService.GetWeather(city, apiKey));
    }

    public WeatherResult GetForecast(string city, int days, string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(city,   nameof(city));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(days, nameof(days));
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey, nameof(apiKey));

        return ExecuteCrossCuttingConcerns(apiKey, city, nameof(GetForecast), ()
            => _realService.GetForecast(city, days, apiKey));
    }
}

// Client — proxy mi gerçek mi bilmiyor
IWeatherService service = new WeatherServiceProxy(new WeatherService());
var result = service.GetWeather("İstanbul", "VALID_KEY");
```

---

## Proxy'nin Pipeline Akışı

| Adım | Sınıf | Başarısız olursa |
|---|---|---|
| 1 | `AuthManager.Authenticate()` | Gerçek servise iletilmez |
| 2 | `RateLimitingManager.CheckRateLimit()` | Gerçek servise iletilmez |
| 3 | `Logger.LogInfo()` | Her zaman çalışır |
| 4 | `_realService.GetWeather/GetForecast()` | Sadece 1 ve 2 geçilirse |

---

## Farkın Özeti

| | Proxy İhlali | Proxy Uyumlu |
|---|---|---|
| Auth kontrolü | Her client'ta tekrar | `AuthManager`'da bir kez |
| Rate limit | Thread-unsafe, her client farklı | `ConcurrentDictionary` ile thread-safe |
| Yeni kontrol | Tüm client'lara dokun | Pipeline'a tek satır ekle |
| Test | Kontrol + iş mantığı iç içe | İzole mock ile test |
| DRY | Her client tekrarlıyor | `ExecuteCrossCuttingConcerns` |

---

## Testler

Testler **xUnit**, **FluentAssertions** ve **Moq** ile yazılmıştır.

### Neden Moq?

Proxy'nin en kritik davranışı şudur: geçersiz API key gelirse gerçek servis hiç çağrılmamalı, rate limit dolunca tam 5 kez çağrılmış olmalı. Bunu doğrulamak için gerçek servisin mock'unu kullanıyoruz ve `Verify` ile kaç kez çağrıldığını kontrol ediyoruz.

### Kapsanan Senaryolar

- Geçerli API key → başarılı sonuç + gerçek servis çağrıldı
- Geçersiz API key → fail + gerçek servis hiç çağrılmadı
- Rate limit dolunca → fail + gerçek servis çağrılmadı
- Rate limit içindeyken → gerçek servis tam 5 kez çağrıldı
- GetForecast geçerli key → başarılı + gerçek servis çağrıldı
- Guard clause'lar — boş city, boş apiKey, 0 ve negatif days
- Constructor null guard
- `IWeatherService` interface'ini implement ediyor

---

## SOLID ile Bağlantısı

| SOLID Prensibi | Bağlantı |
|---|---|
| SRP | `WeatherService` iş mantığı, `AuthManager` auth, `RateLimitingManager` limit, `Logger` log — her sınıf tek sorumluluk |
| OCP | Yeni kontrol (IP block) = yeni sınıf + pipeline'a bir satır, mevcut kod değişmez |
| LSP | Proxy ve WeatherService `IWeatherService` yerine geçebilir — client farkı bilmiyor |
| DIP | Client somut sınıfa değil `IWeatherService` interface'ine bağımlı |
| DRY | `ExecuteCrossCuttingConcerns` pipeline'ı — her metod aynı akışı tekrar yazmıyor |

---

## Diğer Pattern'lerle İlişkisi

**Proxy ↔ Decorator:** İkisi de aynı interface'i implement eder ve gerçek nesneyi sarar. Decorator davranış ekler ve zincirlenebilir — birden fazla Decorator üst üste kullanılabilir. Proxy ise erişimi kontrol eder, client Proxy'nin varlığından habersizdir. Bu projede cross-cutting concerns katmanı Decorator'a yaklaşıyor ama Proxy'nin kimlik gizleme özelliği korunuyor.

**Proxy ↔ Facade:** Facade birden fazla alt sistemi tek API'ye toplar. Proxy tek bir nesneye erişimi kontrol eder, aynı interface'i korur. Facade yeni bir interface sunarken Proxy mevcut interface'i korur.

**Proxy ↔ Adapter:** Adapter farklı interface'leri uyumlu hale getirir. Proxy gerçek nesneyle aynı interface'i korur. Adapter dönüşüm yapar, Proxy kontrol eder.

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Amaç |
|---|---|---|
| .NET | 8.0 | Ana platform |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |
| Moq | 4.20.72 | Mock framework |

---

## 📚 Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Flyweight | Structural | ✅ Tamamlandı |
| 2 | Adapter | Structural | ✅ Tamamlandı |
| 3 | Composite | Structural | ✅ Tamamlandı |
| 4 | Facade | Structural | ✅ Tamamlandı |
| 5 | Proxy | Structural | ✅ Tamamlandı |
| 6 | Decorator | Structural | 🔜 Yakında |
| 7 | Bridge | Structural | 🔜 Yakında |

---