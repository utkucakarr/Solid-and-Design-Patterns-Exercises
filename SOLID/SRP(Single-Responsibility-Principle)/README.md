SOLID #1 — Single Responsibility Principle (SRP)
> *"Bir sınıfın değişmesi için yalnızca bir sebebi olmalıdır."*
> — Robert C. Martin
---
Prensip Nedir?
Single Responsibility Principle, bir sınıfın yalnızca tek bir sorumluluğu olması gerektiğini söyler.
Başka bir deyişle: bir sınıfı değiştirmen gerekiyorsa, bunun için yalnızca tek bir nedenin olması gerekir.
Eğer bir sınıf hem veritabanına yazıyor, hem e-posta gönderiyor, hem de loglama yapıyorsa — bu sınıfın değişmesi için birden fazla sebep var demektir. Bu da beraberinde şu sorunları getirir:
Bir özellik değiştiğinde alakasız kodlar da etkilenir.
Test yazmak zorlaşır.
Kod tekrarı artar.
Bağımlılıklar giderek büyür.
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
Veritabanı motoru değişirse	`Order_Bad` değişmeli.
E-posta servisi değişirse	`Order_Bad` değişmeli.
Log formatı değişirse	`Order_Bad` değişmeli.
Hesaplama mantığı değişirse	`Order_Bad` değişmeli.
Her değişiklik, birbirinden tamamen alakasız kodlara dokunmayı zorunlu kılar. Bu da hataya açık, test edilmesi zor bir yapı oluşturur.
---
Doğru Kullanım — SRP Uyumlu Yapı
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
Veritabanı motoru değişirse	Sadece `OrderRepository`.
E-posta servisi değişirse	Sadece `OrderNotificationService`.
Log formatı değişirse	Sadece `OrderLogger`.
Hesaplama mantığı değişirse	Sadece `Order` modeli.
Her değişiklik izole kalır. Diğer sınıflara dokunmana gerek olmaz.
---
Interface Kullanımı
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
Test edilebilirlik — Mock nesnelerle gerçek bağımlılıklar simüle edilebilir.
Değiştirilebilirlik — `OrderRepository` yerine yarın `MongoOrderRepository` yazılabilir `OrderService` hiç değişmez.
Bağımsızlık — Her katman birbirinden ayrı geliştirilebilir.
---
Testler
Testler xUnit, Moq ve FluentAssertions kütüphaneleriyle yazılmıştır.
Kapsanan Senaryolar
OrderTests — Model davranışı:
Toplam tutarın doğru hesaplanması.
Boş sipariş kalemlerinde sıfır dönmesi.
Farklı fiyat ve miktar kombinasyonları `[Theory]`.
OrderServiceTests — Servis davranışı:
Her bağımlılığın tam olarak bir kez çağrılması.
Çağrı sırası: `Save → SendConfirmation → LogOrderCreated`
Repository hata verdiğinde e-posta ve log çağrılmaması.
OrderRepositoryTests — Repository davranışı:
Kaydedilen siparişin ID ile bulunabilmesi.
Var olmayan ID ile `null` dönmesi.
Birden fazla siparişin ayrı ayrı kaydedilmesi.
