# SOLID #5 — Dependency Inversion Principle (DIP)

> *"Üst seviye modüller, alt seviye modüllere bağımlı olmamalıdır. Her ikisi de soyutlamalara bağımlı olmalıdır."*
> — Robert C. Martin

---

## Prensip Nedir?

Dependency Inversion Principle iki şeyi söyler:

1. **Üst seviye modüller** (iş mantığı) alt seviye modüllere (veritabanı, e-posta, SMS) doğrudan bağımlı olmamalıdır.
2. **Her ikisi de soyutlamalara** (interface) bağımlı olmalıdır — detaylar soyutlamalara bağımlı olur, soyutlamalar detaylara değil.

Bir sınıf kullandığı bağımlılıkları `new` ile doğrudan oluşturuyorsa — o sınıf DIP'i ihlal ediyor demektir. Bağımlılıklar dışarıdan, constructor üzerinden verilmelidir.

---

## Kötü Kullanım — DIP İhlali

`NotificationManager_Bad` somut sınıfları doğrudan `new`'liyor:

```csharp
public class NotificationManager_Bad
{
    // Somut sınıflara sıkı bağımlılık
    private readonly EmailService_Bad _emailService = new();
    private readonly SmsService_Bad   _smsService   = new();
    private readonly PushService_Bad  _pushService  = new();

    public void SendAll(string message)
    {
        _emailService.Send(message);
        _smsService.Send(message);
        _pushService.Send(message);
    }
}
```

### Yeni bir kanal (WhatsApp) eklemek istesek ne olur?

1. `WhatsAppService_Bad` sınıfı oluşturulur.
2. `NotificationManager_Bad` içine **girilir.**
3. Yeni `private field` eklenir.
4. `SendAll()` metoduna yeni satır eklenir.
5. Çalışan koda dokunmak zorunda kalınır.

---

## Doğru Kullanım — DIP Uyumlu Yapı

`INotificationService` interface'i tanımlanıyor, `NotificationManager` bu interface'e bağımlı:

```csharp
public interface INotificationService
{
    string Channel { get; }
    NotificationResult Send(string message);
}
```

`NotificationManager` bağımlılıklarını **constructor üzerinden** alıyor:

```csharp
public class NotificationManager
{
    private readonly IEnumerable<INotificationService> _services;

    // Constructor injection — kim gelirse gelsin çalışır
    public NotificationManager(IEnumerable<INotificationService> services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public IEnumerable<NotificationResult> SendAll(string message)
    {
        foreach (var service in _services)
            yield return service.Send(message);
    }
}
```

### Yeni bir kanal (WhatsApp) eklemek istesek ne olur?

```csharp
// Sadece yeni bir sınıf ekliyoruz — NotificationManager'a hiç dokunmuyoruz!
public class WhatsAppNotificationService : INotificationService
{
    public string Channel => "WHATSAPP";

    public NotificationResult Send(string message)
    {
        Console.WriteLine($"[WHATSAPP] {message}");
        return NotificationResult.Success(Channel, message);
    }
}
```

`NotificationManager` değişmez. Mevcut testler bozulmaz. Riske girilmez.

---

## Farkın Özeti

| | DIP İhlali | DIP Uyumlu |
|---|---|---|
| Bağımlılık oluşturma | Sınıf içinde `new` ile | Constructor üzerinden dışarıdan |
| Yeni kanal eklemek | Mevcut sınıfı değiştir | Yeni sınıf ekle |
| Test yazabilmek | Gerçek servis şart | Mock ile izole test |
| Değiştirilebilirlik | Zor — her yerde `new` var | Kolay — interface değişmez |

---

## Testler

Testler **xUnit**, **Moq** ve **FluentAssertions** ile yazılmıştır.

### Kapsanan Senaryolar

**NotificationManagerTests:**
- `null` servis listesiyle `ArgumentNullException` fırlatılıyor.
- `SendAll` tüm servisleri tam olarak bir kez çağırıyor.
- `SendAll` tüm sonuçları başarılı döndürüyor.
- Boş mesajda `ArgumentException` fırlatılıyor.
- `SendToChannel` sadece ilgili servisi çağırıyor.
- Bilinmeyen kanal için `Fail` sonucu dönüyor.
- **DIP garantisi:** Herhangi bir `INotificationService` implementasyonuyla çalışıyor.
- **DIP garantisi:** Farklı sayıda servisle `NotificationManager` değişmiyor.


