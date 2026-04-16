# Design Patterns #1 — Singleton Pattern

> *"Bir sınıfın yalnızca tek bir instance'ının olmasını garanti eden ve bu instance'a global bir erişim noktası sağlayan tasarım desenidir."*
> — Gang of Four (GoF)

---

## Singleton Pattern Nedir?

Singleton Pattern, bir sınıfın uygulama boyunca **yalnızca tek bir instance** oluşturulmasını garanti eder. Bu instance'a her yerden aynı erişim noktası üzerinden ulaşılır.

**Ne zaman kullanılır?**

- Tüm uygulama boyunca tek bir nesneye ihtiyaç duyulduğunda
- Aynı kaynağa birden fazla yerden erişilmesi gerektiğinde
- Nesne oluşturmanın maliyetli olduğu durumlarda

**Gerçek hayat örnekleri:**
- Log yöneticisi — tüm uygulama tek dosyaya yazar
- Veritabanı bağlantısı — connection pool yönetimi
- Konfigürasyon yöneticisi — ayarlar bir kez okunur
- Cache yöneticisi — bellekte tek bir cache nesnesi

---

## Senaryo
Bir uygulamada tüm katmanlar log üretiyor:

OrderService → sipariş logları
UserService → kullanıcı logları
PaymentService → ödeme logları

Eğer her servis kendi Logger'ını oluştursaydı → aynı anda birden fazla servis aynı dosyaya yazmaya çalışır → dosya çakışması, veri kaybı, tutarsız loglar.
Singleton ile → tüm uygulama tek bir Logger kullanır, loglar sıralı ve tutarlı olur.

---

## Kötü Kullanım — Singleton İhlali

Her servis kendi Logger instance'ını oluşturuyor:

```csharp
public class OrderService_Bad
{
    // Her servis kendi Logger'ını new'liyor
    private readonly Logger_Bad _logger = new("app.log");

    public void CreateOrder(string product)
        => _logger.Log($"Sipariş oluşturuldu: {product}");
}

public class UserService_Bad
{
    // Başka bir Logger instance'ı — aynı dosyaya iki ayrı nesne yazıyor!
    private readonly Logger_Bad _logger = new("app.log");

    public void RegisterUser(string username)
        => _logger.Log($"Kullanıcı kaydedildi: {username}");
}
```

### Sonuçlar:

```
[Logger] Yeni instance oluşturuldu. ID: a1b2c3   ← OrderService
[Logger] Yeni instance oluşturuldu. ID: d4e5f6   ← UserService
[Logger] Yeni instance oluşturuldu. ID: g7h8i9   ← PaymentService
```

| Sorun | Açıklama |
|---|---|
| Dosya çakışması | Aynı anda birden fazla nesne aynı dosyaya yazıyor |
| Kaynak israfı | Her servis için ayrı Logger nesnesi bellekte tutuluyor |
| Tutarsızlık | Her instance farklı ID taşıyor — hangi log nereden geldi? |
| Veri kaybı | Eş zamanlı yazma sırasında log satırları karışabilir |

---

## Doğru Kullanım — Singleton Pattern

Üç temel kural uygulanıyor:

```csharp
public sealed class AppLogger : IAppLogger
{
    // 1. Tek instance burada tutuluyor
    private static AppLogger? _instance;

    // 2. Thread-safe için lock nesneleri
    private static readonly object _lock     = new();
    private static readonly object _fileLock = new();

    // 3. Private constructor — dışarıdan new'lenemez!
    private AppLogger() { }

    // Thread-safe Singleton — double-checked locking
    public static AppLogger GetInstance()
    {
        if (_instance is null)
        {
            lock (_lock)
            {
                if (_instance is null)
                    _instance = new AppLogger();
            }
        }
        return _instance;
    }
}
```

---

## Singleton'ın 3 Temel Kuralı

```
1. Private constructor   → Dışarıdan new'lenemez
2. Static instance       → Tek instance sınıf içinde tutuluyor
3. Static GetInstance()  → Tek erişim noktası
```

---

## Thread-Safety — Double-Checked Locking

Çok iş parçacıklı ortamda iki thread aynı anda `GetInstance()` çağırabilir. Double-checked locking bunu önler:

```csharp
public static AppLogger GetInstance()
{
    if (_instance is null)          // 1. kontrol — lock olmadan (hızlı)
    {
        lock (_lock)                // sadece null ise lock al
        {
            if (_instance is null)  // 2. kontrol — lock içinde (güvenli)
                _instance = new AppLogger();
        }
    }
    return _instance;
}
```

- İlk kontrol lock olmadan yapılır — performanslı
- Instance null ise lock alınır — thread-safe
- Lock içinde tekrar kontrol edilir — race condition önlenir

---

## Testler

Testler **xUnit**, **Moq** ve **FluentAssertions** ile yazılmıştır.

### Kapsanan Senaryolar

**Singleton Garantisi:**
- Birden fazla çağrıda aynı instance döner
- Instance ID'leri eşit olmalı
- Instance null olmamalı

**Thread-Safety:**
- 20 farklı thread'den aynı anda `GetInstance()` çağrılıyor
- Hepsi aynı instance'ı almalı

**Servis Testleri — Mock ile:**
- `OrderService.CreateOrder()` → Info log çağrılıyor
- `OrderService.CancelOrder()` → Warning log çağrılıyor
- `UserService.RegisterUser()` → Info log çağrılıyor
- `UserService.LoginFailed()` → Warning log çağrılıyor
- `PaymentService.ProcessPayment()` → Info log çağrılıyor
- `PaymentService.PaymentFailed()` → Error log çağrılıyor

**Guard Clause:**
- Boş mesaj → `ArgumentException`
- Null logger → `ArgumentNullException`
- Negatif fiyat → `ArgumentOutOfRangeException`

```

## Singleton Ne Zaman Kullanılmamalı?

Singleton güçlü bir pattern ama her yerde kullanmak doğru değil:

| Durum | Neden Uygun Değil |
|---|---|
| Birim test yazılacaksa | Global state testleri zorlaştırır |
| Bağımlılık değişecekse | Tek instance esnekliği azaltır |
| Çok fazla sorumluluk varsa | God Object'e dönüşme riski |

> Bu projede `IAppLogger` interface'i kullanıldığı için servisler mock ile test edilebiliyor — Singleton'ın test edilebilirlik sorunu aşılmış oluyor.

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Singleton | Creational | ✅ Tamamlandı |
| 2 | Factory Method | Creational | 🔜 Yakında |
| 3 | Abstract Factory | Creational | 🔜 Yakında |
| 4 | Prototype | Creational | 🔜 Yakında |
| 5 | Builder | Creational | 🔜 Yakında |

---

Her türlü soru ve geri bildirim için LinkedIn üzerinden ulaşabilirsiniz.
