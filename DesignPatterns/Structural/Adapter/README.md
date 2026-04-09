# Design Patterns #7 — Adapter Pattern

> *"Uyumsuz interface'lere sahip nesnelerin birlikte çalışmasını sağlayan yapısal bir tasarım desenidir."*
> — Gang of Four (GoF)

---

## Adapter Pattern Nedir?

Adapter Pattern, birbirleriyle uyumsuz interface'lere sahip sınıfların birlikte çalışmasını sağlar. Üçüncü parti kütüphaneleri, eski sistemleri veya farklı API'leri mevcut kodla entegre ederken mevcut koda dokunmadan köprü görevi görür.

**Ne zaman kullanılır?**

- Üçüncü parti kütüphanenin interface'i sistemin interface'iyle uyuşmuyorsa
- Eski (legacy) kodu değiştirmeden yeni sistemle entegre etmek gerekiyorsa
- Farklı API'leri tek bir interface arkasında toplamak gerekiyorsa
- Kod tabanı sabit ama dış bağımlılık değişiyorsa

**Gerçek hayat örnekleri:**
- Ödeme entegrasyon sistemi — PayPal, Stripe, Iyzico
- Loglama kütüphaneleri — NLog, Serilog, log4net adaptasyonu
- Veritabanı sürücüleri — farklı DB API'lerini tek interface'e bağlamak
- Harita servisleri — Google Maps, OpenStreetMap adaptasyonu

---

## Kötü Kullanım — Adapter İhlali

Client her sağlayıcının API detaylarını biliyor:

```csharp
public class CheckoutService_Bad
{
    public void ProcessPayment(string provider, decimal amount)
    {
        // Her sağlayıcı için ayrı kod — OCP ihlali!
        if (provider == "paypal")
        {
            var result = _payPal.MakePayment((double)amount, "TRY");
        }
        else if (provider == "stripe")
        {
            var cents  = (int)(amount * 100);  // cent dönüşümü client'ta
            var result = _stripe.Charge(cents, "try", "tok_test");
        }
        else if (provider == "iyzico")
        {
            var result = _iyzico.OdemeYap(amount, "TRY"); // Türkçe API
        }
        // Yeni sağlayıcı = buraya yeni else if!
    }
}
```

### Sonuçlar:

| Sorun | Açıklama |
|---|---|
| OCP ihlali | Yeni sağlayıcı = mevcut koda dokun |
| Sıkı bağımlılık | Client her API'nin detayını biliyor |
| Tekrar eden dönüşümler | decimal → double, decimal → cent her yerde |
| Test edilemezlik | Gerçek ödeme servisi olmadan test yazılamaz |

---

## Doğru Kullanım — Adapter Pattern

Her sağlayıcı için bir Adapter — client sadece interface'i biliyor:

```csharp
// Sistemin kendi interface'i
public interface IPaymentProcessor
{
    string ProviderName { get; }
    PaymentResult ProcessPayment(decimal amount, string currency);
    PaymentResult Refund(string transactionId, decimal amount);
}

// PayPal Adapter — decimal → double dönüşümü burada!
public class PayPalAdapter : IPaymentProcessor
{
    private readonly PayPalService _payPalService;

    public PaymentResult ProcessPayment(decimal amount, string currency)
    {
        // Dönüşüm adapter'da — client bilmek zorunda değil
        var transactionId = _payPalService.MakePayment((double)amount, currency);
        return PaymentResult.Success(transactionId, ProviderName, amount, currency);
    }
}

// Stripe Adapter — decimal → cent dönüşümü burada!
public class StripeAdapter : IPaymentProcessor
{
    public PaymentResult ProcessPayment(decimal amount, string currency)
    {
        // Cent dönüşümü adapter'da — client bilmek zorunda değil
        var cents = (int)(amount * 100);
        _stripeService.Charge(cents, currency, "tok_test");
        return PaymentResult.Success(...);
    }
}
```

Client tüm sağlayıcıları aynı şekilde kullanıyor:

```csharp
// Client sadece IPaymentProcessor biliyor
var processors = new List<IPaymentProcessor>
{
    new PayPalAdapter(new PayPalService()),
    new StripeAdapter(new StripeService()),
    new IyzicoAdapter(new IyzicoService())
};

foreach (var processor in processors)
{
    var result = processor.ProcessPayment(1000, "TRY");
    Console.WriteLine(result.IsSuccess ? $"Succss: {result.Message}" : $"Fail: {result.Message}");
}
```

---

## Adapter'ın Yaptığı Dönüşümler

| Sağlayıcı | Üçüncü Parti API | Adapter'ın Dönüşümü |
|---|---|---|
| PayPal | `MakePayment(double, string)` | `decimal` → `double` |
| Stripe | `Charge(int cents, string, string)` | `decimal` → `int * 100` |
| Iyzico | `OdemeYap(decimal, string)` → `int` | HTTP `200` → `bool` |

---

## Farkın Özeti

| | Adapter İhlali | Adapter Uyumlu |
|---|---|---|
| Yeni sağlayıcı | Mevcut koda dokun | Yeni Adapter sınıfı ekle |
| API dönüşümleri | Client'ta dağınık | Adapter'da merkezi |
| OCP | İhlal | Korunuyor |
| Test edilebilirlik | Gerçek servis şart | Mock ile izole test |
| Client bilgisi | Her API'nin detayını biliyor | Sadece IPaymentProcessor |

---

## Testler

Testler **xUnit** ve **FluentAssertions** ile yazılmıştır.

### Kapsanan Senaryolar

**PayPalAdapterTests:**
- `ProcessPayment()` başarılı sonuç döner
- ProviderName `PayPal` olmalı
- TransactionId boş olmamalı
- Sıfır tutar → `ArgumentOutOfRangeException`
- Boş currency → `ArgumentException`
- `Refund()` başarılı sonuç döner
- `null` servis → `ArgumentNullException`

**StripeAdapterTests:**
- `ProcessPayment()` başarılı sonuç döner
- decimal → cent dönüşümü doğru çalışıyor
- Negatif tutar → `ArgumentOutOfRangeException`
- `Refund()` başarılı sonuç döner

**IyzicoAdapterTests:**
- `ProcessPayment()` başarılı sonuç döner
- HTTP 200 → `IsSuccess = true` dönüşümü doğru
- Boş transactionId → `ArgumentException`
- `Refund()` başarılı sonuç döner

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Amaç |
|---|---|---|
| .NET | 8.0 | Ana platform |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |

---

## 📚 Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Flyweight | Structural | ✅ Tamamlandı |
| 2 | Adapter | Structural | ✅ Tamamlandı |
| 3 | Composite | Structural | 🔜 Yakında |
| 4 | Facade | Structural | 🔜 Yakında |
| 5 | Proxy | Structural | 🔜 Yakında |
| 6 | Decorator | Structural | 🔜 Yakında |
| 7 | Bridge | Structural | 🔜 Yakında |

---
