# Visitor Pattern

> *"Represent an operation to be performed on elements of an object structure. Visitor lets you define a new operation without changing the classes of the elements on which it operates."*
> *"Bir nesne yapısının elemanları üzerinde gerçekleştirilecek bir işlemi temsil eder. Ziyaretçi (Visitor), üzerinde çalıştığı elemanların sınıflarını değiştirmeden yeni bir işlem tanımlamanıza olanak tanır."*
> — **Gang of Four**

---

## Pattern Nedir?

**Visitor (Ziyaretçi)**, bir nesne yapısındaki elemanlar üzerinde yeni operasyonlar tanımlamaya yarayan davranışsal (behavioral) bir tasarım desenidir. Temel fikir şudur: operasyonu elemandan ayır, ayrı bir "ziyaretçi" sınıfına taşı. Bu sayede mevcut sınıfları hiç değiştirmeden yeni davranışlar ekleyebilirsin.

### Ne Zaman Kullanılır?

- Nesne yapısı nadiren değişiyor ama üzerinde sık sık yeni operasyonlar tanımlanması gerekiyorsa
- Farklı tiplerdeki nesneler üzerinde tip bazlı farklı davranış uygulanması gerekiyorsa
- `if-else` veya `switch` ile tip kontrolü yapılıyorsa ve bu yapı büyüyorsa
- Bir nesne hiyerarşisine dokunmadan yeni özellik eklenmesi gerekiyorsa
- Operasyonların nesne sınıflarına yayılmasını engellemek istiyorsan

### Gerçek Hayat Örnekleri

| Alan | Örnek |
|------|-------|
| E-Ticaret | Farklı ürün tiplerine vergi, indirim, rapor uygulama |
| Muhasebe | Farklı hesap tiplerine faiz, vergi, denetim uygulama |
| Derleyici | AST (Abstract Syntax Tree) node'larını gezme ve dönüştürme |
| Belge | Farklı element tiplerine (başlık, paragraf, tablo) dışa aktarım |
| Oyun | Farklı karakter tiplerine hasar, iyileşme, buff uygulama |

---

### Senaryo
"Bir e-ticaret platformunda ürün kataloğu yönetimi geliştiriyoruz. Fiziksel, dijital ve abonelik ürünleri var. Bu ürünler üzerinde vergi hesaplama, indirim uygulama ve rapor üretme gibi operasyonlar çalıştırmamız gerekiyor. Ürün sınıflarını her yeni operasyonda değiştirmek istemiyoruz."

---

## Kötü Kullanım

```csharp
public class CatalogService_Bad
{
    // Her operasyon aynı sınıfta — SRP ihlali
    public decimal CalculateTax(string productType, decimal basePrice, decimal weight = 0)
    {
        // string ile tip kontrolü — tip güvenliği yok
        if (productType == "Physical")
            return basePrice * 0.18m + weight * 0.5m;
        else if (productType == "Digital")
            return basePrice * 0.08m;
        else if (productType == "Subscription")
            return basePrice * 0.18m;
        else
            throw new ArgumentException($"Bilinmeyen tip: {productType}");
    }

    // Aynı if-else zinciri tekrar tekrar — kod tekrarı
    public decimal CalculateDiscount(string productType, decimal basePrice, bool isPremium)
    {
        if (productType == "Physical")
            return isPremium ? basePrice * 0.15m : basePrice * 0.05m;
        else if (productType == "Digital")
            return isPremium ? basePrice * 0.20m : basePrice * 0.10m;
        else if (productType == "Subscription")
            return isPremium ? basePrice * 0.25m : 0m;
        else
            throw new ArgumentException($"Bilinmeyen tip: {productType}");
    }

    // "GiftProduct" eklenince 3 metot birden güncellenmeli — OCP ihlali
    public string GenerateReport(string productType, string name, decimal price)
    {
        if (productType == "Physical")
            return $"[FİZİKSEL] {name} | {price:C} | Kargo: Gerekli";
        else if (productType == "Digital")
            return $"[DİJİTAL] {name} | {price:C} | Anında Teslimat";
        else if (productType == "Subscription")
            return $"[ABONELİK] {name} | Aylık: {price:C}";
        else
            throw new ArgumentException($"Bilinmeyen tip: {productType}");
    }
}
```

### Sonuçlar

| Sorun | Açıklama |
|-------|----------|
| Open/Closed İhlali | Yeni ürün tipi eklenince tüm if-else blokları güncellenir |
| Single Responsibility | Vergi + indirim + rapor tek sınıfta — 3 farklı sorumluluk |
| Tip Güvenliği Yok | `string` ile tip kontrolü — yazım hatası runtime'a kadar fark edilmez |
| Kod Tekrarı | Aynı if-else zinciri her operasyon için kopyalanıyor |
| Bakım Zorluğu | Yeni operasyon = dev sınıfı tekrar aç, tüm tipleri güncelle |

---

## Doğru Kullanım

```csharp
// IProduct — her ürün ziyaretçiyi kabul eder
public interface IProduct
{
    VisitResult Accept(IProductVisitor visitor);
}

// IProductVisitor — her ürün tipi için ayrı overload
public interface IProductVisitor
{
    VisitResult Visit(PhysicalProduct product);
    VisitResult Visit(DigitalProduct product);
    VisitResult Visit(SubscriptionProduct product);
}

// Ürün sınıfı sadece Accept çağırır — başka hiçbir şey bilmez
public sealed class PhysicalProduct : IProduct
{
    public VisitResult Accept(IProductVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor, nameof(visitor));
        return visitor.Visit(this); // ✅ Double dispatch burada gerçekleşir
    }
}

// Yeni operasyon = sadece yeni Visitor sınıfı — hiçbir ürün sınıfı değişmez
public sealed class TaxCalculatorVisitor : IProductVisitor
{
    public VisitResult Visit(PhysicalProduct product)
    {
        var tax = product.BasePrice * 0.18m + product.WeightKg * 0.5m;
        return VisitResult.Success(tax, $"[Vergi] {product.Name}: {tax:C}");
    }

    public VisitResult Visit(DigitalProduct product)
    {
        var tax = product.BasePrice * 0.08m;
        return VisitResult.Success(tax, $"[Vergi] {product.Name}: {tax:C}");
    }

    public VisitResult Visit(SubscriptionProduct product)
    {
        var tax = product.BasePrice * 0.18m * product.DurationMonths;
        return VisitResult.Success(tax, $"[Vergi] {product.Name}: {tax:C}");
    }
}

// Katalog üzerinde ziyaretçiyi çalıştır
foreach (var product in catalog)
{
    var result = product.Accept(taxVisitor); // Double dispatch burada
    Console.WriteLine(result.Message);
}
```

---

## Double Dispatch Nasıl Çalışır?

| Adım | Ne Olur? | Kim Karar Verir? |
|------|----------|-----------------|
| 1 | `product.Accept(visitor)` çağrılır | Caller (Program.cs) |
| 2 | `Accept` içinde `visitor.Visit(this)` çağrılır | Ürün sınıfı (`this` tipi belli) |
| 3 | `Visit(PhysicalProduct)` overload'u seçilir | C# runtime (overload resolution) |
| 4 | Fiziksel ürüne özgü vergi hesaplanır | `TaxCalculatorVisitor` |
| | İki dispatch: ürün tipi + visitor tipi | İkisi birlikte doğru metodu belirler |

---

## Farkın Özeti

| Özellik | Bad (if-else) | Good (Visitor) |
|---------|-----------------|------------------|
| Yeni ürün tipi eklemek | Tüm if-else blokları güncellenir | `IProduct` + `IProductVisitor`'a overload eklenir |
| Yeni operasyon eklemek | Dev sınıfı tekrar açılır | Yeni `Visitor` sınıfı yazılır, hiçbir ürün değişmez |
| Tip güvenliği | `string` ile — runtime hatası | C# overload resolution — derleme zamanı güvenliği |
| Kod tekrarı | Her operasyonda aynı if-else | Her visitor kendi sorumluluğunda — tekrar yok |
| Test edilebilirlik | Her şey iç içe — izole test zor | Her visitor bağımsız test edilir |
| Single Responsibility | 3 sorumluluk tek sınıfta | Her visitor tek operasyondan sorumlu |

---

## Testler

### Neden Moq Kullanılmadı?

Visitor pattern'inde her `Visit` overload'u **deterministik hesaplamalar** yapıyor. Ürün sınıfları dış bağımlılık içermiyor — sadece immutable property'ler taşıyor. Bu nedenle `new TaxCalculatorVisitor()` ile izole test yeterli. Eğer visitor içinde `IEmailService` veya `IRepository` gibi dış bağımlılık olsaydı Moq devreye girerdi.

### Kapsanan Senaryolar

| Kategori | Test Sayısı | Örnek |
|----------|-------------|-------|
| TaxCalculatorVisitor happy path | 4 | KDV + ağırlık, indirimli KDV, ay bazlı, ağır ürün |
| DiscountVisitor happy path | 7 | Premium/standart × 3 tip + karşılaştırma |
| ReportVisitor happy path | 3 | Her ürün tipi için rapor içeriği |
| Double dispatch doğrulama | 2 | Aynı visitor farklı tipler, katalog iterasyonu |
| VisitResult factory metodları | 3 | Success / Report / Fail |
| Guard clause — ürün konstruktörleri | 6 | Null/boş name, negatif fiyat, ağırlık, süre |
| Null guard — Accept | 3 | Her ürün null visitor'a karşı |
| Null guard — Visit overload'ları | 9 | Her visitor × her ürün tipi null product |
| **Toplam** | **35** | |

---

## SOLID ile Bağlantısı

| Prensip | Açıklama |
|---------|----------|
| **S** — Single Responsibility | Her visitor sınıfı tek bir operasyondan sorumludur |
| **O** — Open/Closed | Yeni operasyon = yeni visitor — mevcut ürün sınıfları değişmez |
| **L** — Liskov Substitution | `IProductVisitor` yerine herhangi bir visitor kullanılabilir |
| **I** — Interface Segregation | `IProduct` sadece `Accept`, `IProductVisitor` sadece `Visit` overload'larını içerir |
| **D** — Dependency Inversion | Ürünler `IProductVisitor` arayüzüne bağlıdır, somut visitor'a değil |

---

## Diğer Pattern'lerle İlişkisi

| Pattern | İlişki |
|---------|--------|
| **Composite** | Visitor genellikle Composite ağacını gezmek için kullanılır — derleyici AST'leri bunun klasik örneğidir |
| **Iterator** | Visitor, koleksiyonu gezerken her elemana uygulanır — ikisi birlikte sıkça kullanılır |
| **Strategy** | Strategy çalışma zamanında algoritma değiştirir; Visitor ise nesne tipine göre farklı davranır |
| **Chain of Responsibility (09)** | CoR isteği zincirde filtreler; Visitor ise nesne yapısında operasyon uygular |
| **Adapter (07)** | Adapter arayüz dönüştürür; Visitor operasyon ekler — ikisi farklı sorun çözer |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| C# | 12 | Dil — overload resolution ile double dispatch |
| .NET | 8 | Platform |
| Microsoft.Extensions.DependencyInjection | 10.0.7 | DI container |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |

---

##  Design Patterns Serisi

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
| 9 | Visitor | Behavioral | ✅ Tamamlandı |
| 10 | State | Behavioral | 🔜 Yakında |