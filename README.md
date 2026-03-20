🧱 SOLID #1 — Single Responsibility Principle (SRP)
> *"Bir sınıfın değişmesi için yalnızca bir sebebi olmalıdır."*
> — Robert C. Martin
---
Prensip Nedir?
Single Responsibility Principle, bir sınıfın yalnızca tek bir sorumluluğu olması gerektiğini söyler.
Başka bir deyişle: bir sınıfı değiştirmen gerekiyorsa, bunun için yalnızca tek bir nedenin olması gerekir.
Eğer bir sınıf hem veritabanına yazıyor, hem e-posta gönderiyor, hem de loglama yapıyorsa — bu sınıfın değişmesi için birden fazla sebep var demektir. Bu da beraberinde şu sorunları getirir:
Bir özellik değiştiğinde alakasız kodlar da etkilenir
Test yazmak zorlaşır
Kod tekrarı artar
Bağımlılıklar giderek büyür
---
Kötü Kullanım — SRP İhlali
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
✅Doğru Kullanım — SRP Uyumlu Yapı
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
Testler
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

## Prensip Nedir?

Open/Closed Principle, mevcut ve çalışan bir koda **dokunmadan** yeni davranışlar ekleyebilmemiz gerektiğini söyler.

- **Açık (Open)** → Yeni özellikler eklenebilmeli
- **Kapalı (Closed)** → Mevcut kod değiştirilmemeli

Bir sınıfa yeni bir özellik eklemek için içine girip mevcut kodu düzenlemek zorundaysak — o sınıf OCP'yi ihlal ediyor demektir. Her düzenleme, mevcut testleri bozma ve yeni hata ekleme riskini beraberinde getirir.

---

## Kötü Kullanım — OCP İhlali

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

## Doğru Kullanım — OCP Uyumlu Yapı

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

## Farkın Özeti

| | OCP İhlali | OCP Uyumlu |
|---|---|---|
| Yeni indirim eklemek | Mevcut sınıfı değiştir | Yeni sınıf ekle |
| Mevcut testler | Bozulabilir | Dokunulmaz |
| Kod değişikliği riski | Her seferinde var | Sıfır |
| Genişletme yöntemi | `switch-case` ekle | Yeni `IDiscountStrategy` |

---

## Testler

Testler **xUnit** ile yazılmıştır.

### Kapsanan Senaryolar

**DiscountCalculatorTests** — Hesaplayıcı davranışı:
- Standart indirimde farklı tutarlar için doğru sonuç `[Theory]`
- Premium indirimde doğru yüzde hesabı `[Fact]`

# 🔄 SOLID #3 — Liskov Substitution Principle (LSP)

> *"Alt sınıflar, üst sınıflarının yerine geçtiğinde program doğru çalışmaya devam etmelidir."*
> — Barbara Liskov, 1987

---

## Prensip Nedir?

Liskov Substitution Principle, bir alt sınıfın üst sınıfının **tüm sözleşmelerini eksiksiz yerine getirmesi** gerektiğini söyler.

E�er bir alt sınıf, üst sınıfın bir metodunu `NotSupportedException` fırlatarak geçersiz kılıyorsa — bu LSP ihlalidir. Çünkü o sınıf, üst sınıfın yerine güvenle **geçemiyor** demektir.

**Test sorusu:** Üst sınıf referansını alt sınıfla değiştirdiğinde program hâlâ doğru çalışıyor mu?

- Evet → LSP uyumlu
- Hayır / Exception fırlatıyor → LSP ihlali

## Kötü Kullanım — LSP İhlali

`Employee_Bad` abstract sınıfı, tüm çalışan tiplerine aynı sözleşmeyi dayatıyor:

```csharp
public abstract class Employee_Bad
{
    public abstract decimal CalculateSalary();
    public abstract decimal CalculateBonus();    // Her çalışan prim alabilir mi? HAYIR!
    public abstract decimal CalculateOvertime(); // Her çalışan fazla mesai alabilir mi? HAYIR!
}
```

Stajyer ve sözleşmeli çalışanlar bu sözleşmeyi yerine getiremiyor:

```csharp
public class Intern_Bad : Employee_Bad
{
    public override decimal CalculateBonus()
        => throw new NotSupportedException("Stajyerler prim alamaz!");

    public override decimal CalculateOvertime()
        => throw new NotSupportedException("Stajyerler fazla mesai alamaz!");
}
```

### Sonuç: Runtime'da patlıyor!

```csharp
var employees = new List<Employee_Bad>
{
    new FullTimeEmployee_Bad("Ahmet", 30000),
    new Contractor_Bad("Ayşe", 20000),
    new Intern_Bad("Mehmet", 10000)  
};

foreach (var emp in employees)
    emp.CalculateBonus();
```

`Intern_Bad`, `Employee_Bad`'in yerine geçtiğinde program çöküyor. **Bu direkt LSP ihlalidir.**

---

## Doğru Kullanım — LSP Uyumlu Yapı

Her yetenek ayrı bir interface ile temsil ediliyor. Çalışanlar **yalnızca hak ettikleri sözleşmeleri** implemente ediyor:

```csharp
public interface IEmployee        { decimal CalculateSalary(); }
public interface IBonusEligible   { decimal CalculateBonus(); }
public interface IOvertimeEligible { decimal CalculateOvertime(); }
```

```csharp
// Tüm çalışanlar — sadece maaş, güvenli!
var allEmployees = new List<IEmployee> { fullTime, contractor, intern };

// Sadece prim alabilenler — Intern giremez, compiler engeller!
var bonusEligibles = new List<IBonusEligible> { fullTime, contractor };

// Sadece fazla mesai alabilenler
var overtimeEligibles = new List<IOvertimeEligible> { fullTime };
```

Artık `Intern` yanlış listeye **giremez bile.** Hata runtime'da değil, **derleme anında** yakalanır.

---

## Farkın Özeti

| | LSP İhlali | LSP Uyumlu |
|---|---|---|
| Hata zamanı | Runtime — program çalışırken patlıyor | Compile time — derleme anında yakalanıyor |
| Yanlış kullanım | Liste içine girebilir, fark edilmez | Compiler engeller, listeye giremez |
| Sözleşme | Herkese aynı — yerine getirilemiyor | Her sınıf sadece söz verdiğini yapıyor |
| Güvenlik | try-catch ile kapatmak gerekiyor | Tip sistemi koruyor |

---

## Testler

Testler **xUnit** ve **FluentAssertions** ile yazılmıştır.

### Kapsanan Senaryolar

**FullTimeEmployeeTests:**
- Maaş, prim ve fazla mesai doğru hesaplanıyor
- `IEmployee`, `IBonusEligible`, `IOvertimeEligible` atanabilir
- Guard clause — boş isim ve negatif maaş engelleniyor

**InternTests:**
- Maaş doğru hesaplanıyor
- `IEmployee` atanabilir
- `IBonusEligible` ve `IOvertimeEligible` **atanamıyor** — LSP garantisi
- `IEmployee` referansıyla kullanıldığında exception yok

**ContractorTests:**
- Maaş ve prim doğru hesaplanıyor
- `IEmployee` ve `IBonusEligible` atanabilir
- `IOvertimeEligible` **atanamıyor** — LSP garantisi

## SOLID Serisi

| # | Prensip | Durum |
|---|---|---|
| 1 | Single Responsibility Principle | Tamamlandı |
| 2 | Open/Closed Principle | Tamamlandı |
| 3 | Liskov Substitution Principle | Tamamlandı |
| 4 | Interface Segregation Principle | Bu repo |
| 5 | Dependency Inversion Principle | Yakında |

