# Design Patterns #11 — Decorator Pattern

> *"Bir nesneye, aynı sınıftaki diğer nesneleri etkilemeksizin, dinamik olarak yeni sorumluluklar ekleyen yapısal bir tasarım desenidir."*
> — Gang of Four (GoF)

---

## Decorator Pattern Nedir?

Decorator Pattern, bir nesnenin davranışını değiştirmeden üzerine yeni sorumluluklar ekleyen yapısal bir tasarım desenidir. Kalıtım yerine kompozisyonu tercih eder — her Decorator, sarmaldığı nesneyle aynı interface'i implement eder ve zincirlenebilir.

**Ne zaman kullanılır?**

- Bir nesneye isteğe bağlı ve birleştirilebilir özellikler eklemek gerekiyorsa
- Kalıtım ile class explosion riski varsa (N özellik = 2^N sınıf)
- Çalışma zamanında davranış değiştirmek gerekiyorsa
- Cross-cutting concern'leri esnek biçimde uygulamak gerekiyorsa

**Gerçek hayat örnekleri:**
- E-posta servisi — sıkıştırma, şifreleme, loglama katmanları
- .NET Stream sınıfları — `GZipStream(CryptoStream(FileStream(...)))`
- ASP.NET Core Middleware — her middleware zincirde bir decorator
- HTTP Client handler'ları — retry, loglama, auth handler'ları

---

## Senaryo
E-posta Gönderim Servisi. Temel e-posta gönderimi üzerine sıkıştırma, şifreleme ve loglama katmanları eklenmesi gerekiyor. Her katman isteğe bağlı ve zincirlenebilir olmalı — client hangi katmanları istediğini kendisi seçebilmeli.

---


## Kötü Kullanım — Decorator İhlali

Her özellik kombinasyonu için ayrı sınıf yazılıyor — class explosion:

```csharp
//  3 özellik = 8 farklı sınıf kombinasyonu!
public class BasicEmailService_Bad { ... }
public class CompressedEmailService_Bad { ... }
public class EncryptedEmailService_Bad { ... }
public class LoggedEmailService_Bad { ... }
public class CompressedEncryptedEmailService_Bad { ... }
public class CompressedLoggedEmailService_Bad { ... }
public class EncryptedLoggedEmailService_Bad { ... }
public class CompressedEncryptedLoggedEmailService_Bad { ... }
// 4. özellik (retry) eklersen = 16 sınıf!
// Gönderim kodu her sınıfta tekrar yazılıyor — DRY ihlali
// Çalışma zamanında kombinasyon değiştirilemez
```

### Sonuçlar:

| Sorun | Açıklama |
|---|---|
| Class explosion | N özellik = 2^N kombinasyon sınıfı |
| DRY ihlali | Temel gönderim kodu her sınıfta tekrar |
| OCP ihlali | Yeni özellik = tüm kombinasyonlara dokun |
| Esneksizlik | Çalışma zamanında kombinasyon değiştirilemez |

---

## Doğru Kullanım — Decorator Pattern

Her katman bağımsız, zincirlenebilir, isteğe bağlı:

```csharp
// Abstract Decorator — tüm decorator'ların base class'ı
public abstract class BaseEmailDecorator : IEmailService
{
    protected readonly IEmailService _innerService;

    protected BaseEmailDecorator(IEmailService innerService)
        => _innerService = innerService
               ?? throw new ArgumentNullException(nameof(innerService));

    public virtual EmailResult Send(EmailMessage message)
        => _innerService.Send(message); // Default: inner'a ilet
}

// Her decorator tek sorumluluk ekliyor
public class CompressionEmailDecorator : BaseEmailDecorator
{
    public override EmailResult Send(EmailMessage message)
    {
        message.Body = Compress(message.Body); // Önce sıkıştır
        return _innerService.Send(message);    // Sonra inner'a ilet
    }
}

// Client istediği kombinasyonu seçiyor — runtime'da
IEmailService basic     = new EmailService();
IEmailService withAll   = new LoggingEmailDecorator(
                          new EncryptionEmailDecorator(
                          new CompressionEmailDecorator(
                          new EmailService())));

// Her ikisi de IEmailService — client fark gözetmiyor
basic.Send(message);    // Sadece temel gönderim
withAll.Send(message);  // Log → Şifrele → Sıkıştır → Gönder
```

---

## Zincirleme Sırası

| Dış → İç | Yürütme Sırası |
|---|---|
| `Logging → Encryption → Compression → Base` | Log başlar → Şifrele → Sıkıştır → Gönder → Log biter |
| `Compression → Base` | Sıkıştır → Gönder |
| `Encryption → Compression → Base` | Şifrele → Sıkıştır → Gönder |

---

## Farkın Özeti

| | Decorator İhlali | Decorator Uyumlu |
|---|---|---|
| Sınıf sayısı | 2^N kombinasyon | N decorator + 1 base |
| Yeni özellik | Tüm kombinasyonlara dokun | Yeni decorator sınıfı ekle |
| Çalışma zamanı | Kombinasyon sabitleniyor | İstediğin kombinasyonu seç |
| DRY | Temel kod her sınıfta tekrar | Base'de bir kez |
| Test | Her kombinasyon ayrı test | Her decorator izole test |

---

## Testler

Testler **xUnit**, **FluentAssertions** ve **Moq** ile yazılmıştır.

### Test Stratejisi

Her decorator **izole** test edildi — inner servis mock'landı. Ayrıca `DecoratorChainTests` ile gerçek zincirleme davranışı ve uygulama **sırası** doğrulandı.

### Kapsanan Senaryolar

**CompressionEmailDecoratorTests:**
- Başarılı sonuç döner
- Inner servise delege eder
- Body sıkıştırılmış olarak inner servise gider
- `AppliedDecorators` listesinde `"Compression"` var
- Boş body → `ArgumentException`
- Constructor null guard

**EncryptionEmailDecoratorTests:**
- Body Base64 şifrelenmiş olarak inner servise gider
- Decode edilen body orijinalle eşleşiyor
- `AppliedDecorators` listesinde `"Encryption"` var

**LoggingEmailDecoratorTests:**
- Inner servis başarısız olunca fail dönüyor
- `AppliedDecorators` listesinde `"Logging"` var

**DecoratorChainTests:**
- Tekli decorator çalışıyor
- Çift decorator çalışıyor
- Tam zincir (3 decorator) çalışıyor
- Decorator'lar doğru sırada uygulanıyor: `Base → Compression → Encryption → Logging`

---


## Diğer Pattern'lerle İlişkisi

**Decorator ↔ Proxy:** İkisi de aynı interface'i implement eder ve gerçek nesneyi sarar. Decorator davranış **ekler** ve zincirlenebilir — client bilerek seçer. Proxy erişimi **kontrol eder** ve genellikle tek katmandır — client Proxy'nin varlığından habersizdir.

**Decorator ↔ Composite:** Composite ağaç yapısı oluşturur — bileşen ve yaprak aynı interface. Decorator ise doğrusal zincir oluşturur — her halka bir öncekini sarar.

**Decorator ↔ Chain of Responsibility:** İkisi de zincir yapısı kullanır. Chain of Responsibility'de zincirdeki her halka isteği işleyip işlememeye **karar verir** ve zinciri durdurabilir. Decorator'da her halka her zaman işler ve bir sonrakine **iletir**.

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Flyweight | Structural | ✅ Tamamlandı |
| 2 | Adapter | Structural | ✅ Tamamlandı |
| 3 | Composite | Structural | ✅ Tamamlandı |
| 4 | Facade | Structural | ✅ Tamamlandı |
| 5 | Proxy | Structural | ✅ Tamamlandı |
| 6 | Decorator | Structural | ✅ Tamamlandı |
| 7 | Bridge | Structural | 🔜 Yakında |

---
