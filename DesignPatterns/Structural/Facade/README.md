# Design Patterns #8 — Facade Pattern

> *"Bir alt sistemdeki arayüzler kümesine birleşik bir arayüz sağlar. Facade, alt sistemi kullanmayı kolaylaştıran üst düzey bir arayüz tanımlar."*
> — Gang of Four (GoF)

---

## Facade Pattern Nedir?

Facade Pattern, karmaşık bir alt sistem bütününe basitleştirilmiş tek bir arayüz sağlayan yapısal bir tasarım desenidir. Client, alt sistemlerle doğrudan konuşmak yerine yalnızca Facade ile konuşur — tüm orchestration mantığı ve alt sistem detayları Facade'ın arkasında gizlenir.

**Ne zaman kullanılır?**

- Client'ın birden fazla alt sistemi sıralı olarak çağırması gerekiyorsa
- İş mantığının (orchestration) client'a sızmasını önlemek gerekiyorsa
- Karmaşık bir sisteme basit bir API sunmak gerekiyorsa
- Katmanlı mimaride katmanlar arası bağımlılığı azaltmak gerekiyorsa

**Gerçek hayat örnekleri:**
- Banka para transfer sistemi — bakiye, fraud, transfer, bildirim, audit
- Kullanıcı kayıt akışı — validasyon, DB kaydı, e-posta, audit log
- E-ticaret sipariş sistemi — stok, ödeme, fatura, kargo, bildirim
- CI/CD pipeline — build, test, image oluştur, deploy, bildirim

---


## Kötü Kullanım — Facade İhlali

Client transfer akışının tüm adımlarını kendisi yönetiyor:

```csharp
public class BankingClient_Bad
{
    public void Transfer(string fromAccount, string toAccount, decimal amount)
    {
        // Client tüm adımları sıralıyor!
        var balance = _balance.GetBalance(fromAccount);         // 1
        if (_fraud.IsSuspicious(fromAccount, amount)) return;  // 2
        _balance.DeductBalance(fromAccount, amount);           // 3
        var refId = _transfer.ExecuteTransfer(...);            // 4
        _balance.AddBalance(toAccount, amount);                // 5
        _notification.NotifySender(...);                       // 6
        _notification.NotifyReceiver(...);                     // 7
        _audit.LogTransfer(...);                               // 8
        // Yeni adım = bu methoda dokun!
        // Rollback yok — bakiye düştü ama transfer patlarsa?
    }
}
```

### Sonuçlar:

| Sorun | Açıklama |
|---|---|
| İş mantığı sızdı | Adım sırası ve kurallar client'ta |
| Yüksek bağımlılık | Client 5 alt sistemi doğrudan biliyor |
| Rollback yok | Bakiye düşüp transfer patlarsa para kaybolur |
| Kod tekrarı | Her banking client aynı akışı yeniden yazar |

---

## Doğru Kullanım — Facade Pattern

Client sadece `BankingFacade.Transfer()` çağırıyor:

```csharp
// Facade — tüm karmaşıklık burada gizli
public class BankingFacade
{
    public TransferResult Transfer(string fromAccount, string toAccount, decimal amount)
    {
        if (_balanceService.GetBalance(fromAccount) < amount)
            return TransferResult.Fail("Yetersiz bakiye.");

        if (_fraudService.IsSuspicious(fromAccount, amount))
            return TransferResult.Fail("Şüpheli işlem tespit edildi.");

        _balanceService.DeductBalance(fromAccount, amount);
        var referenceId = _transferService.ExecuteTransfer(fromAccount, toAccount, amount);
        _balanceService.AddBalance(toAccount, amount);
        _notificationService.NotifySender(fromAccount, amount, referenceId);
        _notificationService.NotifyReceiver(toAccount, amount, referenceId);
        _auditService.LogTransfer(fromAccount, toAccount, amount, referenceId);

        return TransferResult.Success(referenceId, fromAccount, toAccount, amount);
    }
}

// Client sadece bunu biliyor:
var result = facade.Transfer("ACC_001", "ACC_002", 2500);
Console.WriteLine(result.IsSuccess ? $"Success: {result.Message}" : $"Fail: {result.Message}");
```

---

## Facade'ın Yönettiği Akış

| Adım | Alt Sistem | Sorumluluk |
|---|---|---|
| 1 | BalanceService | Bakiye yeterliliğini kontrol et |
| 2 | FraudService | Şüpheli işlem kontrolü yap |
| 3 | BalanceService | Göndericiden bakiyeyi düş |
| 4 | TransferService | Transferi gerçekleştir |
| 5 | BalanceService | Alıcıya bakiyeyi ekle |
| 6 | NotificationService | Gönderici ve alıcıya bildir |
| 7 | AuditService | İşlemi logla |

Client bu 7 adımın hiçbirini bilmiyor — sadece `Transfer()` biliyor.

---

## Farkın Özeti

| | Facade İhlali | Facade Uyumlu |
|---|---|---|
| Client bilgisi | 5 alt sistemi biliyor | Sadece BankingFacade biliyor |
| Yeni akış adımı | Her client'a dokun | Sadece Facade güncellenir |
| Rollback | Client'ta yönetmek zor | Facade'da merkezi yönetim |
| Test | 5 servis gerekli | Mock ile izole test |
| Sorumluluk | Client'ta dağınık | Facade'da merkezi |

---

## Testler

Testler **xUnit**, **FluentAssertions** ve **Moq** ile yazılmıştır.

### Neden Moq?

Facade'ın görevi alt sistemleri doğru sırayla çağırmaktır. Fraud tespit edilirse bakiye düşmemeli, bakiye yetersizse transfer başlatılmamalı. Bu orchestration kurallarını gerçek servisler olmadan doğrulamak için Moq kullanıyoruz.

### Kapsanan Senaryolar

- `Transfer()` başarılı sonuç döner
- ReferenceId ve Amount doğru dönmeli
- Bakiye kontrolü doğru hesapla çağrılmalı
- Fraud kontrolü doğru parametrelerle çağrılmalı
- Gönderici bakiyesi düşmeli, alıcı bakiyesi artmalı
- Gönderici ve alıcıya bildirim gitmeli
- Audit log yazılmalı
- Yetersiz bakiyede transfer ve bildirim olmamalı
- Fraud tespit edilince bakiye düşmemeli ve audit log atılmamalı
- Guard clause'lar — boş hesap, 0 ve negatif tutar
- Constructor null guard'ları

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Amaç |
|---|---|---|
| .NET | 8.0 | Ana platform |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |
| Moq | 4.20.72 | Mock framework |

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Flyweight | Structural | ✅ Tamamlandı |
| 2 | Adapter | Structural | ✅ Tamamlandı |
| 3 | Composite | Structural | ✅ Tamamlandı |
| 4 | Facade | Structural | ✅ Tamamlandı |
| 5 | Proxy | Structural | 🔜 Yakında |
| 6 | Decorator | Structural | 🔜 Yakında |
| 7 | Bridge | Structural | 🔜 Yakında |

---