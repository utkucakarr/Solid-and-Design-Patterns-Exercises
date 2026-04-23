# Chain of Responsibility Pattern

> *"Avoid coupling the sender of a request to its receiver by giving more than one object a chance to handle the request. Chain the receiving objects and pass the request along the chain until an object handles it."*
> *"İsteği işlemek için birden fazla nesneye fırsat tanıyarak, isteğin göndericisi ile alıcısı arasındaki bağımlılığı (coupling) önleyin. Alıcı nesneleri bir zincir haline getirin ve içlerinden bir nesne onu işleyene kadar isteği bu zincir boyunca iletin."*
> — **Gang of Four**

---

## Pattern Nedir?

**Chain of Responsibility (Sorumluluk Zinciri)**, bir isteğin sırayla birden fazla nesneye iletildiği ve her nesnenin isteği işleyip işlemeyeceğine karar verdiği davranışsal (behavioral) bir tasarım desenidir.

### Ne Zaman Kullanılır?

- Bir isteği işleyecek nesnenin önceden bilinmediği durumlarda
- Birden fazla nesnenin aynı isteği işleyebileceği durumlarda
- İşlem adımlarının sırasının dinamik olarak değişebildiği durumlarda
- Her adımın bağımsız olarak genişletilebilir ve test edilebilir olması gerektiğinde
- Gönderici ile alıcı arasındaki bağı kesmek istediğinizde

### Gerçek Hayat Örnekleri

| Alan | Örnek |
|------|-------|
| E-Ticaret | Sipariş onayı: stok → fraud → ödeme → kargo |
| Güvenlik | Authentication middleware zinciri |
| İş Akışı | Belge onayı: çalışan → müdür → direktör |
| Web | ASP.NET Core Middleware pipeline |
| Oyun | Hasar hesaplama: zırh → kalkan → can |

---

## Senaryo
"Bir e-ticaret platformunda sipariş onay akışı geliştiriyoruz. Her sipariş onaylanmadan önce sırasıyla stok, fraud, ödeme ve kargo kontrollerinden geçmeli. Bu kontrollerin bağımsız, sıralanabilir ve genişletilebilir olmasını istiyoruz."

---

## Kötü Kullanım

```csharp
public class CheckoutService_Bad
{
    public string ProcessOrder(string productId, int quantity,
                               string customerId, decimal accountBalance)
    {
        // ADIM 1: Stok kontrolü burada
        var stock = GetStock(productId);
        if (stock < quantity)
            return $"Stok yetersiz. Mevcut: {stock}";

        // ADIM 2: Fraud kontrolü burada
        var fraudScore = GetFraudScore(customerId);
        if (fraudScore > 75)
            return $"Fraud skoru: {fraudScore}";

        // ADIM 3: Ödeme kontrolü burada
        var total = GetPrice(productId) * quantity;
        if (accountBalance < total)
            return $"Bakiye yetersiz. Gereken: {total}";

        // ADIM 4: Kargo kontrolü burada
        if (!CheckShipping(productId))
            return "Kargo mevcut değil";

        return  Sipariş onaylandı!";
    }
}
```

### Sonuçlar

| Sorun | Açıklama |
|-------|----------|
| Open/Closed İhlali | Her yeni adım bu metodu açmayı gerektirir |
| Single Responsibility | Tüm sorumluluklar tek sınıfta toplanmış |
| Test Edilemezlik | Her adımı izole test etmek imkânsız |
| Sıra Değişikliği | Adım sırasını değiştirmek metodu yeniden yazmayı gerektirir |
| Tight Coupling | Fraud atlanıp doğrudan ödemeye geçilemez |

---

## Doğru Kullanım

```csharp
// Her handler sadece kendi işinden sorumlu
public sealed class FraudHandler : BaseOrderHandler
{
    private const decimal FraudThreshold = 75m;

    public override OrderResult Handle(OrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var score = GetFraudScore(request.CustomerId);

        if (score > FraudThreshold)
            return OrderResult.Fail(
                $"Şüpheli işlem. Fraud skoru: {score}",
                nameof(FraudHandler));

        Console.WriteLine($"  [FraudHandler] Fraud skoru normal: {score}");
        return PassToNext(request); // Zincir devam eder
    }
}

// Zinciri fluent API ile kur
stockHandler
    .SetNext(fraudHandler)
    .SetNext(paymentHandler)
    .SetNext(shippingHandler);

var result = stockHandler.Handle(request);
```

---

## Pattern'in Yaptığı İşlem

| Adım | Handler | Kontrol | Başarısız Olursa |
|------|---------|---------|-----------------|
| 1 | `StockHandler` | Stokta yeterli ürün var mı? | Zincir kırılır, `StockHandler` döner |
| 2 | `FraudHandler` | Fraud skoru ≤ 75 mi? | Zincir kırılır, `FraudHandler` döner |
| 3 | `PaymentHandler` | Bakiye yeterli mi? | Zincir kırılır, `PaymentHandler` döner |
| 4 | `ShippingHandler` | Kargo oluşturulabilir mi? | Zincir kırılır, `ShippingHandler` döner |
| | — | Tüm adımlar geçildi | Tracking code üretilir, `Success` döner |

---

## Farkın Özeti

| Özellik | Bad (Tek Metot) | Good (Zincir) |
|---------|-------------------|-----------------|
| Yeni adım eklemek | Mevcut sınıfı aç, değiştir | Yeni class yaz, zincire ekle |
| Adım sırası | Sabit, değiştirmek riskli | Fluent API ile kolayca değişir |
| Test edilebilirlik | Her adımı izole test etmek imkânsız | Her handler bağımsız test edilir |
| Tek sorumluluk | Tüm sorumluluklar bir arada | Her handler tek iş yapar |
| Hangi adımda kırıldı | Bilinmiyor | `FailedHandler` alanında görünür |
| A/B test / konfigürasyon | İmkânsız | Zinciri runtime'da kurabilirsin |

---

## Testler

### Neden Moq Kullanılmadı?

Chain of Responsibility handler'ları **gerçek Dictionary tabanlı veriler** üzerinden çalıştığından ve her handler **deterministik** sonuçlar ürettiğinden, Moq'a gerek duyulmadı. Her handler izole olarak `new StockHandler()` ile test edilebilmektedir.

### Kapsanan Senaryolar

| Kategori | Test Sayısı | Örnek |
|----------|-------------|-------|
| Happy path | 2 | Tüm adımlar geçilir, tracking code üretilir |
| Stok handler | 3 | Stok yetersiz, ürün bulunamadı, miktar aşıldı |
| Fraud handler | 3 | Fraud tespit, fraud geçti, sonraki handler çağrılmadı |
| Payment handler | 2 | Bakiye yetersiz, tam bakiye geçer |
| Shipping handler | 1 | Dijital ürün kargo reddedildi |
| Zincir sırası | 2 | Stok kırılınca fraud çağrılmaz, tek handler success |
| Guard clause | 4 | Null/boş productId, customerId, negatif quantity, bakiye |
| Null guard | 5 | Her handler null request atar, SetNext null atar |
| Result nesnesi | 2 | Success/Fail factory metodları doğru property set eder |
| **Toplam** | **28** | |

---

## SOLID ile Bağlantısı

| Prensip | Açıklama |
|---------|----------|
| **S** — Single Responsibility | Her handler yalnızca bir kontrolden sorumludur |
| **O** — Open/Closed | Yeni handler eklemek mevcut kodu değiştirmez |
| **L** — Liskov Substitution | `BaseOrderHandler` yerine herhangi bir alt sınıf kullanılabilir |
| **I** — Interface Segregation | `IOrderHandler` yalnızca `SetNext` ve `Handle` içerir |
| **D** — Dependency Inversion | Handler'lar `IOrderHandler` arayüzüne bağlıdır, somut sınıflara değil |

---

## Diğer Pattern'lerle İlişkisi

| Pattern | İlişki |
|---------|--------|
| **Decorator** | Her ikisi de nesneleri zincirler; Decorator davranış ekler, CoR isteği iletir veya durdurur |
| **Command** | İstekler Command nesnesi olarak paketlenip zincire gönderilebilir |
| **Composite** | Handler'lar Composite yapısıyla ağaç şeklinde organize edilebilir |
| **Facade (08)** | Facade karmaşık sistemi gizler; CoR ise isteği adım adım filtreler |
| **Adapter (07)** | Adapter arayüz dönüştürür; CoR isteği zincir boyunca yönlendirir |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| C# | 12 | Dil |
| .NET | 8 | Platform |
| Microsoft.Extensions.DependencyInjection | 8.x | DI container |
| xUnit | 2.x | Test framework |
| FluentAssertions | 6.x | Okunabilir assertion'lar |

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Strategy | Behavioral | ✅ Tamamlandı |
| 2 | Command | Behavioral | ✅ Tamamlandı |
| 3 | Iterator | Behavioral | ✅ Tamamlandı |
| 4 | Template Metot | Behavioral | ✅ Tamamlandı |
| 5 | Observer | Behavioral | ✅ Tamamlandı |
| 6 | Memento | Behavioral | ✅ Tamamlandı |
| 7 | Mediator | Behavioral | ✅ Tamamlandı |
| 8 | Chain Of Responsibility | Behavioral | ✅ Tamamlandı |
| 9 | Visitor | Behavioral | 🔜 Yakında |
| 10 | State | Behavioral | 🔜 Yakında |