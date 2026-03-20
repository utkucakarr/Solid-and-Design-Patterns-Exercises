🧱 SOLID #1 — Single Responsibility Principle (SRP)
> *"Bir sınıfın değişmesi için yalnızca bir sebebi olmalıdır."*
> — Robert C. Martin
---
📌 Prensip Nedir?
Single Responsibility Principle, bir sınıfın yalnızca tek bir sorumluluğu olması gerektiğini söyler.
Başka bir deyişle: bir sınıfı değiştirmen gerekiyorsa, bunun için yalnızca tek bir nedenin olması gerekir.
Eğer bir sınıf hem veritabanına yazıyor, hem e-posta gönderiyor, hem de loglama yapıyorsa — bu sınıfın değişmesi için birden fazla sebep var demektir. Bu da beraberinde şu sorunları getirir:
Bir özellik değiştiğinde alakasız kodlar da etkilenir
Test yazmak zorlaşır
Kod tekrarı artar
Bağımlılıklar giderek büyür
---
❌ Kötü Kullanım — SRP İhlali
`Order_Bad` sınıfı tek başına 4 farklı iş yapıyor:
```csharp
public class Order_Bad
{
    public decimal CalculateTotal() { ... }    // 1. Hesaplama
    public void SaveToDatabase()    { ... }    // 2. Veritabanı
    public void SendConfirmationEmail() { ... } // 3. E-posta
    public void LogOrder()          { ... }    // 4. Loglama
}
```
Neden sorunlu?
Değişiklik Sebebi	Etkilenen Yer
Veritabanı motoru değişirse	`Order_Bad` değişmeli
E-posta servisi değişirse	`Order_Bad` değişmeli
Log formatı değişirse	`Order_Bad` değişmeli
Hesaplama mantığı değişirse	`Order_Bad` değişmeli
Her değişiklik, birbirinden tamamen alakasız kodlara dokunmayı zorunlu kılar. Bu da hataya açık, test edilmesi zor bir yapı oluşturur.
---
✅ Doğru Kullanım — SRP Uyumlu Yapı
Her sınıfa tek bir sorumluluk verildi:
```
Order              → Sadece sipariş verisi ve toplam hesaplama
OrderRepository    → Sadece veritabanı işlemleri
OrderNotificationService → Sadece e-posta gönderimi
OrderLogger        → Sadece loglama
OrderService       → Tüm adımları bir araya getiren orkestratör
```
```csharp
// Her sınıf yalnızca kendi işini yapıyor
public class OrderService
{
    public void ProcessOrder(Order order)
    {
        _repository.Save(order);                    // DB işi repository'de
        _notificationService.SendConfirmation(order); // Mail işi serviste
        _logger.LogOrderCreated(order);             // Log işi logger'da
    }
}
```
Artık ne değişirse ne olur?
Değişiklik Sebebi	Etkilenen Yer
Veritabanı motoru değişirse	Sadece `OrderRepository`
E-posta servisi değişirse	Sadece `OrderNotificationService`
Log formatı değişirse	Sadece `OrderLogger`
Hesaplama mantığı değişirse	Sadece `Order` modeli
Her değişiklik izole kalır. Diğer sınıflara dokunmana gerek olmaz.
---
🔌 Interface Kullanımı
Servisler doğrudan sınıflara değil, interface'lere bağımlıdır:
```csharp
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly IOrderLogger _logger;
}
```
Bu yaklaşımın faydaları:
Test edilebilirlik — Mock nesnelerle gerçek bağımlılıklar simüle edilebilir
Değiştirilebilirlik — `OrderRepository` yerine yarın `MongoOrderRepository` yazılabilir, `OrderService` hiç değişmez
Bağımsızlık — Her katman birbirinden ayrı geliştirilebilir
---
🧪 Testler
Testler xUnit, Moq ve FluentAssertions kütüphaneleriyle yazılmıştır.
Kapsanan Senaryolar
OrderTests — Model davranışı:
Toplam tutarın doğru hesaplanması
Boş sipariş kalemlerinde sıfır dönmesi
Farklı fiyat ve miktar kombinasyonları `[Theory]`
OrderServiceTests — Servis davranışı:
Her bağımlılığın tam olarak bir kez çağrılması
Çağrı sırası: `Save → SendConfirmation → LogOrderCreated`
Repository hata verdiğinde e-posta ve log çağrılmaması
OrderRepositoryTests — Repository davranışı:
Kaydedilen siparişin ID ile bulunabilmesi
Var olmayan ID ile `null` dönmesi
Birden fazla siparişin ayrı ayrı kaydedilmesi

# 🔓 SOLID #2 — Open/Closed Principle (OCP)

> *"Yazılım varlıkları (sınıflar, modüller, fonksiyonlar) genişlemeye açık, değişime kapalı olmalıdır."*
> — Robert C. Martin

---

## 📌 Prensip Nedir?

Open/Closed Principle, mevcut ve çalışan bir koda **dokunmadan** yeni davranışlar ekleyebilmemiz gerektiğini söyler.

- **Açık (Open)** → Yeni özellikler eklenebilmeli
- **Kapalı (Closed)** → Mevcut kod değiştirilmemeli

Bir sınıfa yeni bir özellik eklemek için içine girip mevcut kodu düzenlemek zorundaysak — o sınıf OCP'yi ihlal ediyor demektir. Her düzenleme, mevcut testleri bozma ve yeni hata ekleme riskini beraberinde getirir.

---

## ❌ Kötü Kullanım — OCP İhlali

`DiscountManager` sınıfı içinde `switch-case` kullanılıyor:

```csharp
public decimal CalculateDiscount(decimal amount, CustomerType customerType)
{
    switch (customerType)
    {
        case CustomerType.Standard: return amount * 0.05m;
        case CustomerType.Premium:  return amount * 0.10m;
        case CustomerType.VIP:      return amount * 0.20m;
        default: throw new ArgumentException("Invalid customer type");
    }
}
```

### Yeni bir müşteri tipi (örn: Student) eklemek istesek ne olur?

1. `CustomerType` enum'una `Student` eklenir
2. `DiscountManager` sınıfının **içine girilir**
3. Yeni bir `case` eklenir
4. Tüm mevcut testler yeniden çalıştırılır
5. Başka `switch-case` kullanan yerler de güncellenir

Her yeni gereksinim, çalışan kodu değiştirmeyi zorunlu kılar. Bu da mevcut davranışları bozma riskini sürekli canlı tutar.

---

## ✅ Doğru Kullanım — OCP Uyumlu Yapı

`IDiscountStrategy` interface'i ile her indirim tipi **bağımsız bir sınıf** olarak tanımlanıyor:

```csharp
public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal amount);
}
```

```csharp
public class StandartDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal amount) => amount * 0.05m;
}
0
public class PremiumDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal amount) => amount * 0.10m;
}

public class VipDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal amount) => amount * 0.20m;
}
```

`DiscountCalculator` sınıfı ise **hiç değişmiyor:**

```csharp
public decimal Calculate(decimal amount, IDiscountStrategy strategy)
{
    if (amount <= 0)
        return 0;

    return strategy.ApplyDiscount(amount);
}
```

### Yeni bir müşteri tipi (örn: Student) eklemek istesek ne olur?

```csharp
// Sadece yeni bir sınıf ekliyoruz — başka hiçbir yere dokunmuyoruz!
public class StudentDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal amount) => amount * 0.15m;
}
```

`DiscountCalculator` değişmez. Mevcut testler bozulmaz. Riske girilmez.

---

## 🔑 Farkın Özeti

| | OCP İhlali | OCP Uyumlu |
|---|---|---|
| Yeni indirim eklemek | Mevcut sınıfı değiştir | Yeni sınıf ekle |
| Mevcut testler | Bozulabilir | Dokunulmaz |
| Kod değişikliği riski | Her seferinde var | Sıfır |
| Genişletme yöntemi | `switch-case` ekle | Yeni `IDiscountStrategy` |

---

## 🧪 Testler

Testler **xUnit** ile yazılmıştır.

### Kapsanan Senaryolar

**DiscountCalculatorTests** — Hesaplayıcı davranışı:
- Standart indirimde farklı tutarlar için doğru sonuç `[Theory]`
- Premium indirimde doğru yüzde hesabı `[Fact]`

╔══════════════════════════════════════╗
📚 SOLID Serisi
Bu çalışma SOLID prensiplerini ele alan serinin bir parçasıdır:
#	Prensip	Durum
1	Single Responsibility Principle	✅ Bu repo
2	Open/Closed Principle	✅ Bu repo
3	Liskov Substitution Principle	🔜 Yakında
4	Interface Segregation Principle	🔜 Yakında
5	Dependency Inversion Principle	🔜 Yakında
---


Motor Testi: DiscountCalculator sınıfının, kendisine verilen herhangi bir stratejiyi doğru şekilde tetikleyip sonuç döndürdüğü [Theory] ve [InlineData] senaryoları ile doğrulanmıştır.
