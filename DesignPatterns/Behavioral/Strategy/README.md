# Strategy Pattern — Kargo Fiyatlandırma

> *"Bir algoritma ailesi tanımlayın, her birini kapsülleyin (sarmalayın) ve birbirlerinin yerine kullanılabilir hale getirin. Strateji deseni, algoritmaların onları kullanan istemcilerden bağımsız olarak değişebilmesine olanak tanır."*
> — **Gang of Four**

---

## Strategy Pattern Nedir?

**Strategy Pattern**, bir işi yapmanın birden fazla yolunu (algoritmasını) ayrı sınıflara koyarak, bu algoritmalar arasında çalışma zamanında (runtime) geçiş yapılmasını sağlayan bir **davranışsal (behavioral)** tasarım desenidir.

### Ne Zaman Kullanılır?

- Aynı işlemi farklı algoritmalarla yapman gerektiğinde (fiyatlandırma, sıralama, ödeme, sıkıştırma)
- Uzayan `if/else` veya `switch` zincirlerini kırmak istediğinde
- Algoritmanın runtime'da değişmesi gerektiğinde (flash sale, A/B testi)
- Her algoritmanın bağımsız test edilmesi gerektiğinde
- Yeni algoritma eklerken mevcut kodu değiştirmeden genişletmek istediğinde

### Gerçek Hayat Örnekleri

| Domain | Strateji Kullanımı |
|---|---|
| E-ticaret | Kargo fiyatlandırma algoritmaları |
| Ödeme sistemleri | Kredi kartı, havale, kripto ödeme |
| Navigasyon | En hızlı / en kısa / yaya rotası |
| Sıkıştırma | ZIP, RAR, GZIP algoritmaları |
| Sıralama | QuickSort, MergeSort, BubbleSort |
| Oyun AI | Saldırgan, savunmacı, pasif davranış |

---

## Kötü Kullanım

```csharp
public class ShippingService_Bad
{
    public ShippingResult CalculateShipping(ShippingOrder order)
    {
        // Tüm algoritmalar tek metotta — yeni tip = bu sınıf değişir
        if (order.ShippingType == "standard")
        {
            decimal cost = 15.00m + (decimal)order.WeightKg * 2.50m;
            return new ShippingResult { IsSuccess = true, Cost = cost, ... };
        }
        else if (order.ShippingType == "express")
        {
            decimal cost = 35.00m + (decimal)order.WeightKg * 5.00m;
            return new ShippingResult { IsSuccess = true, Cost = cost, ... };
        }
        else if (order.ShippingType == "member")
        {
            // Üyelik kontrolü ve indirim burada gömülü
            if (order.MembershipType != "premium")
                return new ShippingResult { IsSuccess = false, ... };
            decimal cost = (15.00m + (decimal)order.WeightKg * 2.50m) * 0.60m;
            return new ShippingResult { IsSuccess = true, Cost = cost, ... };
        }
        // Yeni kargo tipi = else if zincirine ekleme zorunluluğu
    }
}
```

### Sonuçlar

| Sorun | Açıklama |
|---|---|
| OCP İhlali | Yeni kargo tipi eklemek mevcut sınıfı değiştiriyor |
| SRP İhlali | Tek sınıf tüm algoritma sorumluluklarını taşıyor |
| Test edilemez | Algoritmalar birbirinden izole değil |
| Runtime değişim yok | Çalışırken strateji değiştirme imkânsız |
| Hardcoded sabitler | İndirim oranı, eşik değeri kodun içinde |

---

## Doğru Kullanım

```csharp
// Her algoritma kendi sınıfında
public sealed class MemberShippingStrategy : IShippingStrategy
{
    private const decimal DiscountRate = 0.40m;

    public string StrategyName => "Premium Üye Kargosu";

    public ShippingResult Calculate(ShippingOrder order)
    {
        if (!order.MembershipType.Equals("premium", StringComparison.OrdinalIgnoreCase))
            return ShippingResult.Fail("Bu kargo tipi yalnızca premium üyelere özeldir.");

        decimal cost = (15.00m + (decimal)order.WeightKg * 2.50m) * (1 - DiscountRate);
        return ShippingResult.Success(Math.Round(cost, 2), "PTT Kargo", 4, StrategyName);
    }
}

// Context algoritmayı bilmez, sadece çalıştırır
public sealed class ShippingContext : IShippingContext
{
    private IShippingStrategy? _strategy;

    public void SetStrategy(IShippingStrategy strategy) => _strategy = strategy;

    public ShippingResult ExecuteShipping(ShippingOrder order)
    {
        if (_strategy is null)
            return ShippingResult.Fail("Strateji belirlenmemiş.");

        return _strategy.Calculate(order); // ✅ Polymorphism
    }
}

// Yeni kargo tipi eklemek → sadece yeni sınıf yaz, başka hiçbir şeye dokunma
public sealed class SameDayShippingStrategy : IShippingStrategy { ... }
```

---

## Pattern'in Yaptığı İşlem

| Adım | Açıklama |
|---|---|
| Interface tanımla | `IShippingStrategy` → `Calculate()` sözleşmesi |
| Algoritmaları ayır | Her kargo tipi kendi sınıfına taşınır |
| Context oluştur | `ShippingContext` stratejiyi tutar ve çalıştırır |
| Strateji enjekte et | `SetStrategy()` ile runtime'da değiştirilebilir |
| Çalıştır | Context `Calculate()` çağırır, algoritma bilmez |

---

## Farkın Özeti

| Özellik | Bad (if/else) | Good (Strategy) |
|---|---|---|
| Yeni algoritma ekleme | Mevcut sınıf değişir | Yeni sınıf yaz, dokunma |
| Test edilebilirlik | İzole test imkânsız | Her strateji bağımsız test edilir |
| Runtime değişim | Yok | `SetStrategy()` ile anlık |
| Sorumluluk dağılımı | Tek sınıfta yığılmış | Her sınıf tek algoritma |
| Kod okunurluğu | Uzayan if/else | Küçük, odaklı sınıflar |

---

## Testler

### Neden Moq?

`IShippingStrategy` bir interface olduğundan context testi için gerçek strateji sınıflarına gerek kalmaz. Moq sayesinde `ShippingContext`'in stratejiyi doğru çağırıp çağırmadığı izole test edilir.

### Kapsanan Senaryolar

| Kategori | Test Sayısı | Açıklama |
|---|---|---|
| Happy path | 4 | Her strateji için başarılı hesaplama |
| Başarısız senaryo | 2 | Eşik altı sipariş, standart üye |
| Context davranışı | 3 | Delegasyon, strateji değişimi, boş strateji |
| Guard clause | 12 | Null order, boş OrderId, sıfır ağırlık (4 strateji × 3) |
| Runtime değişim | 1 | Flash sale senaryosu |
| **Toplam** | **22+** | |

---

## Diğer Pattern'lerle İlişkisi

| Pattern | İlişki |
|---|---|
| **Template Method** | Strateji composition ile algoritmayı değiştirir; Template Method ise inheritance ile adımları override eder. İkisi de algoritma değişimine odaklanır ama mekanizma farklıdır. |
| **Factory / Factory Method** | Hangi stratejinin kullanılacağı çoğunlukla bir Factory ile belirlenir. Strategy + Factory kombinasyonu üretim kodunda yaygındır. |
| **Decorator** | Decorator mevcut davranışa katman ekler; Strategy ise davranışın tamamını değiştirir. İkisi birlikte kullanılabilir: strateji seçilir, decorator ile zenginleştirilir. |
| **State** | State Pattern da runtime'da davranışı değiştirir fakat geçişler nesnenin kendi iç durumuna bağlıdır. Strategy'de geçiş dışarıdan tetiklenir. |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|---|---|---|
| C# / .NET | 8.0 | Dil ve platform |
| Microsoft.Extensions.DependencyInjection | 8.x | DI container |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |
| Moq | 4.20.72 | Interface mock'lama |

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Strategy | Behavioral | ✅ Tamamlandı |
| 2 | Command | Behavioral | 🔜 Yakında |
| 3 | Iterator | Behavioral | 🔜 Yakında |
| 4 | Template Metot | Behavioral | 🔜 Yakında |
| 5 | Observer | Behavioral | 🔜 Yakında |
| 6 | Memento | Behavioral | 🔜 Yakında |
| 7 | Mediator | Behavioral | 🔜 Yakında |
| 8 | Chain Of Responsibility | Behavioral | 🔜 Yakında |
| 9 | Visitor | Behavioral | 🔜 Yakında |
| 10 | State | Behavioral | 🔜 Yakında |