# Design Patterns #2 — Factory Method Pattern

> *"Nesne oluşturmak için bir interface tanımlar, ancak hangi sınıfın örnekleneceğine alt sınıfların karar vermesine izin verir."*
> — Gang of Four (GoF)

---

##Factory Method Pattern Nedir?

Factory Method Pattern, nesne oluşturma sorumluluğunu client'tan alıp factory sınıflarına devreder. Client hangi sınıfın oluşturulduğunu bilmek zorunda kalmaz — sadece interface üzerinden çalışır.

**Ne zaman kullanılır?**

- Hangi nesnenin oluşturulacağına çalışma zamanında karar verildiğinde
- Nesne oluşturma mantığı karmaşıklaştığında ve merkezi bir yerden yönetilmesi gerektiğinde
- Yeni tipler eklendiğinde mevcut koda dokunmak istenmediğinde

**Gerçek hayat örnekleri:**
- Bildirim sistemi — Email, SMS, Push kanalları
- Ödeme sistemi — Kredi kartı, havale, kripto
- Rapor üretici — PDF, Excel, Word formatları
- Veritabanı bağlantısı — SQL, MongoDB, Redis

---

## Kötü Kullanım — Factory Method İhlali

`NotificationServiceBad` içinde if-else zinciri ile nesne oluşturuluyor:

```csharp
public void Send(string type, string recipient, string message)
{
    if (type == "email")
    {
        var notification = new EmailNotification_Bad(); 
        notification.Send(recipient, message);
    }
    else if (type == "sms")
    {
        var notification = new SmsNotification_Bad();   
        notification.Send(recipient, message);
    }
    else if (type == "push")
    {
        var notification = new PushNotification_Bad();  
        notification.Send(recipient, message);
    }
}
```

### WhatsApp kanalı eklemek istesek ne olur?

1. `WhatsAppNotificationBad` sınıfı oluşturulur
2. `NotificationServiceBad` içine **girilir**
3. Yeni `else if` bloğu eklenir
4. Çalışan koda dokunmak zorunda kalınır — **OCP ihlali!**

---

## Doğru Kullanım — Factory Method Pattern

Her bildirim tipi için ayrı bir factory sınıfı:

```csharp
// Sözleşmeler
public interface INotification        { NotificationResult Send(string recipient, string message); }
public interface INotificationFactory { INotification CreateNotification(); }

// Her factory kendi nesnesini üretiyor
public class EmailNotificationFactory : INotificationFactory
{
    public INotification CreateNotification() => new EmailNotification();
}

public class SmsNotificationFactory : INotificationFactory
{
    public INotification CreateNotification() => new SmsNotification();
}
```

Client nesne oluşturma detayından tamamen habersiz:

```csharp
// Client sadece interface üzerinden çalışıyor
INotificationFactory factory = new EmailNotificationFactory();
INotification notification   = factory.CreateNotification();

notification.Send("utku@example.com", "Siparişiniz kargoya verildi.");
```

### WhatsApp kanalı eklemek istesek ne olur?

```csharp
// Sadece iki yeni sınıf — mevcut hiçbir koda dokunulmaz!
public class WhatsAppNotification : INotification { ... }
public class WhatsAppNotificationFactory : INotificationFactory
{
    public INotification CreateNotification() => new WhatsAppNotification();
}
```

## Farkın Özeti

| | Factory Method İhlali | Factory Method Uyumlu |
|---|---|---|
| Nesne oluşturma | Client içinde if-else | Factory sınıfında |
| Yeni tip eklemek | Mevcut kodu değiştir | Yeni factory sınıfı ekle |
| OCP | İhlal | Korunuyor |
| Test edilebilirlik | Zor — somut sınıflara bağımlı | Kolay — interface mock'lanabilir |
| Sorumluluk | Client hem iş yapar hem nesne oluşturur | Her sınıf tek iş yapar |

---

## Testler

Testler **xUnit** ve **FluentAssertions** ile yazılmıştır.

### Kapsanan Senaryolar

**EmailNotificationFactoryTests:**
- `CreateNotification()` → `EmailNotification` tipinde nesne döner
- `CreateNotification()` → `INotification` interface'ine atanabilir
- Her çağrıda yeni instance üretiliyor
- Channel değeri `EMAIL` olmalı
- Başarılı send sonucu döner
- Boş recipient/message → `ArgumentException`

**SmsNotificationFactoryTests:**
- `CreateNotification()` → `SmsNotification` tipinde nesne döner
- Channel değeri `SMS` olmalı
- Başarılı send sonucu döner

**PushNotificationFactoryTests:**
- `CreateNotification()` → `PushNotification` tipinde nesne döner
- Channel değeri `PUSH` olmalı
- Her çağrıda yeni instance üretiliyor
- Başarılı send sonucu döner

```

## 📚 Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Singleton | Creational | ✅ Tamamlandı |
| 2 | Factory Method | Creational | ✅ Tamamlandı |
| 3 | Observer | Behavioral | 🔜 Bu repo |
| 4 | Strategy | Behavioral | 🔜 Yakında |
| 5 | Decorator | Structural | 🔜 Yakında |

---

Her türlü soru ve geri bildirim için LinkedIn üzerinden ulaşabilirsiniz.
