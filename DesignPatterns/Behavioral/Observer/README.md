# Observer Pattern — E-Ticaret Sipariş Durumu Bildirimleri

> *"Define a one-to-many dependency between objects so that when one object changes state, all its dependents are notified and updated automatically."*
> *"Nesneler arasında bire çok bir bağımlılık tanımlayın; böylece bir nesnenin durumu değiştiğinde, ona bağlı olan tüm nesneler otomatik olarak bilgilendirilir ve güncellenir."*
> — **Gang of Four, Design Patterns**

---

## Pattern Nedir?

**Observer Pattern**, bir nesnenin (Subject/Observable) durum değiştiğinde kendisine bağlı tüm nesneleri (Observer) otomatik olarak haberdar ettiği davranışsal bir tasarım kalıbıdır. Subject, Observer'ların kim olduğunu veya ne yaptığını bilmez; yalnızca bir liste tutar ve değişiklik olduğunda herkesi bilgilendirir.

### Ne Zaman Kullanılır?

- Bir nesnenin durumu değiştiğinde birden fazla nesnenin tepki vermesi gerektiğinde
- Bildirim gönderen nesnenin, kimin dinlediğini bilmemesi gerektiğinde
- Observer listesinin runtime'da dinamik olarak değişebildiği durumlarda
- Loose coupling (gevşek bağlılık) sağlamak istediğinizde
- Event-driven mimari veya pub-sub sistemi kurulduğunda

### Gerçek Hayat Örnekleri

| Domain | Subject | Observer'lar |
|---|---|---|
| E-Ticaret | Sipariş Servisi | Email, SMS, Push, Fatura, Stok |
| Sosyal Medya | Gönderi Servisi | Takipçi bildirimleri, Feed güncellemesi |
| Borsa | Hisse Senedi Fiyatı | Alım/Satım alarmları, Dashboard |
| CI/CD | Pipeline | Slack bildirimi, Email, Log |
| IoT | Sensör | Dashboard, Alarm, Veri kaydı |

---

## Senaryo

> Bir e-ticaret platformunda sipariş durumu değiştiğinde (Placed → Confirmed → Shipped → Delivered) müşteriye e-posta, SMS ve push bildirimi gönderilmesi, stok güncellenmesi ve fatura oluşturulması gerekiyor. Bu sistemi nasıl tasarlarsın?

---

## Kötü Kullanım (Violation)

```csharp
public void ChangeOrderStatus(string orderId, OrderStatus newStatus)
{
    var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
    order.Status = newStatus;

    // Her durum için if/else zinciri büyümeye devam eder
    if (newStatus == OrderStatus.Confirmed)
    {
        // E-posta, SMS, stok hepsi burada
        Console.WriteLine($"[EMAIL] Siparişiniz onaylandı.");
        Console.WriteLine($"[SMS] Siparişiniz onaylandı.");
        Console.WriteLine($"[INVENTORY] Stok rezerve edildi.");
    }
    else if (newStatus == OrderStatus.Shipped)
    {
        // Yeni durum → yeni if bloğu → şişen metot
        Console.WriteLine($"[EMAIL] Siparişiniz kargoya verildi.");
        Console.WriteLine($"[SMS] Siparişiniz kargoya verildi.");
        Console.WriteLine($"[PUSH] Siparişiniz yola çıktı!");
    }
    // WhatsApp eklemek? Her if bloğuna elle yazılacak!
}
```

### Violation Sonuçları

| Sorun | Açıklama |
|---|---|
| SRP İhlali | `OrderService_Bad` hem durumu değiştiriyor hem de tüm bildirimleri gönderiyor |
| OCP İhlali | Yeni kanal eklemek için mevcut metot değiştirilmeli |
| Test edilemezlik | `Console.WriteLine` ile doğrudan yazıldığı için mock'lanamaz |
| Yüksek bağımlılık | Tüm kanallar `OrderService_Bad`'e sıkı bağlı |
| Bakım zorluğu | Her yeni durum tüm if bloklarını büyütür |

---

## Doğru Kullanım (Implementation)

```csharp
// Subject — Observer'ların ne yaptığını bilmez
public class OrderService : IOrderSubject, IOrderService
{
    private readonly List<IOrderObserver> _observers = new();

    public void Subscribe(IOrderObserver observer) => _observers.Add(observer);
    public void Unsubscribe(IOrderObserver observer) => _observers.Remove(observer);

    public void NotifyObservers(Order order, OrderStatus previousStatus)
    {
        // Tek satır — observer sayısından bağımsız
        foreach (var observer in _observers)
            observer.OnOrderStatusChanged(order, previousStatus);
    }

    public OrderResult ChangeOrderStatus(string orderId, OrderStatus newStatus)
    {
        var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
        var previousStatus = order.Status;
        order.UpdateStatus(newStatus);

        // OrderService sadece notify eder — geri kalanı observer'lar halleder
        NotifyObservers(order, previousStatus);
        return OrderResult.Success(order.OrderId, order.Status, _observers.Count);
    }
}

// Observer — Yalnızca kendi sorumluluğunu bilir
public class EmailNotificationObserver : IOrderObserver
{
    public void OnOrderStatusChanged(Order order, OrderStatus previousStatus)
    {
        // Sadece e-posta işlemi burada
        _emailNotifier.Send(order.CustomerEmail, subject, body);
    }
}
```

---

## Pattern'in Yaptığı İşlem

| Adım | Aktör | Eylem |
|---|---|---|
| 1 | `Program.cs` | Observer'ları `Subscribe` ile kayıt eder |
| 2 | `OrderService` | `ChangeOrderStatus` ile durumu günceller |
| 3 | `OrderService` | `NotifyObservers` çağırır |
| 4 | `EmailObserver` | `OnOrderStatusChanged` → e-posta gönderir |
| 5 | `SmsObserver` | `OnOrderStatusChanged` → SMS gönderir |
| 6 | `PushObserver` | `OnOrderStatusChanged` → push gönderir (koşula göre) |
| 7 | `InventoryObserver` | `OnOrderStatusChanged` → stok rezerve/iade eder |
| 8 | `InvoiceObserver` | `OnOrderStatusChanged` → fatura oluşturur (koşula göre) |

---

## Farkın Özeti

| Özellik | Bad | Good |
|---|---|---|
| Yeni kanal ekleme | `OrderService_Bad` değiştirilmeli | Yeni sınıf yaz, subscribe et |
| SRP | Tüm sorumluluklar tek sınıfta | Her observer tek sorumlu |
| OCP | Mevcut kod değiştirilmeli | Mevcut kod değişmez |
| Test edilebilirlik | Mock'lanamaz | Her observer ayrı test edilebilir |
| Runtime esneklik | Statik bağımlılık | Subscribe/Unsubscribe dinamik |
| Bağımlılık | Sıkı bağlı (tight coupling) | Gevşek bağlı (loose coupling) |
| Kod karmaşıklığı | if/else cehennemi | Temiz, küçük sınıflar |

---

## Testler

### Neden Moq?

Observer Pattern'de `IOrderObserver`, `IEmailNotifier`, `ISmsNotifier` gibi arayüzler gerçek implementasyon olmadan test edilmeli. Moq ile:
- Observer'ların `OnOrderStatusChanged` çağrıldığını doğrularız
- Gerçek e-posta/SMS göndermeden bildirim akışını test ederiz
- `Times.Once`, `Times.Never` ile çağrı sayısı doğrulanır

### Kapsanan Senaryolar

| Kategori | Test Sayısı | Örnekler |
|---|---|---|
| Happy Path | 3 | PlaceOrder başarılı, status değişimi, observer count |
| Observer Çağrı Doğrulama | 5 | Notify çağrısı, unsubscribe sonrası sessizlik |
| Email Observer | 4 | Confirmed, Shipped, Delivered, Cancelled |
| SMS Observer | 2 | Confirmed'da gönderim, Placed'de sessizlik |
| Push Observer | 2 | Shipped'de gönderim, Confirmed'da sessizlik |
| Inventory Observer | 3 | Rezerve, iade, Shipped'de işlem yok |
| Invoice Observer | 2 | Delivered'da fatura, Shipped'de fatura yok |
| Başarısız Senaryolar | 3 | Sipariş bulunamadı, aynı durum, notify yok |
| Guard Clause | 5 | Null order, boş orderId, whitespace |
| Constructor Null Guard | 8 | Tüm observer'lar + Order modeli |

---

## SOLID ile Bağlantısı

| Prensip | Bağlantı |
|---|---|
| **S**RP | Her observer tek bir bildirim kanalından sorumlu |
| **O**CP | Yeni observer eklemek için mevcut kod değiştirilmez |
| **L**SP | Tüm `IOrderObserver` implementasyonları birbirinin yerine geçebilir |
| **I**SP | `IOrderSubject` ve `IOrderService` ayrı arayüzlerde tutuldu |
| **D**IP | `OrderService` somut sınıflara değil, `IOrderObserver` arayüzüne bağımlı |

---

## Diğer Pattern'lerle İlişkisi

| Pattern | İlişki |
|---|---|
| **Mediator** | Observer'da Subject doğrudan Observer'ları tetikler. Mediator'da merkezi bir arabulucu vardır; nesneler birbirini bilmez. Observer daha basit, Mediator daha kontrollüdür. |
| **Event Aggregator** | Observer'ın mimariye yansıması. .NET'in `event/delegate` yapısı veya `IObservable<T>` / `IObserver<T>` arayüzleri Observer Pattern'in built-in uygulamasıdır. |
| **Chain of Responsibility** | Her ikisi de bir tetikleyiciye yanıt verir; ancak CoR'da yalnızca bir işleyici yanıt verirken Observer'da tüm aboneler yanıt verir. |
| **Strategy** | Observer ile birlikte kullanılabilir: her observer farklı bir bildirim stratejisi uygulayabilir. |
| **Facade (8. Pattern)** | Facade karmaşık alt sistemleri tek arayüzde toplarken, Observer bir olayı birden fazla bağımsız sisteme yayar. |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|---|---|---|
| C# / .NET | 8.0 | Ana geliştirme platformu |
| Microsoft.Extensions.DependencyInjection | 10.0.6 | DI container |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |
| Moq | 4.20.72 | Observer/Notifier mock'lama |

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Strategy | Behavioral | ✅ Tamamlandı |
| 2 | Command | Behavioral | ✅ Tamamlandı |
| 3 | Iterator | Behavioral | ✅ Tamamlandı |
| 4 | Template Metot | Behavioral | ✅ Tamamlandı |
| 5 | Observer | Behavioral | ✅ Tamamlandı |
| 6 | Memento | Behavioral | 🔜 Yakında |
| 7 | Mediator | Behavioral | 🔜 Yakında |
| 8 | Chain Of Responsibility | Behavioral | 🔜 Yakında |
| 9 | Visitor | Behavioral | 🔜 Yakında |
| 10 | State | Behavioral | 🔜 Yakında |
