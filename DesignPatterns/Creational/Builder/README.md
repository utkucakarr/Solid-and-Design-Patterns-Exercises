# Design Patterns #5 — Builder Pattern

> *"Karmaşık bir nesnenin inşasını onun temsilinden ayırır; böylece aynı inşa süreci farklı temsiller oluşturabilir."*
> — Gang of Four (GoF)

---

## Builder Pattern Nedir?

Builder Pattern, çok sayıda parametreye sahip karmaşık nesneleri **adım adım** inşa etmeyi sağlar. Telescoping Constructor sorununu çözer — uzun parametre listesi yerine anlamlı, okunabilir method chaining kullanılır.

**Ne zaman kullanılır?**

- Nesnenin çok fazla parametresi varsa ve hepsini constructor'a geçmek okunaksız hale geliyorsa
- Aynı inşa süreciyle farklı konfigürasyonlar oluşturulacaksa
- Nesne oluşturma adımlarının belirli bir sıraya sokulması gerekiyorsa
- Zorunlu ve opsiyonel parametreler arasında net bir ayrım yapılması gerekiyorsa

**Gerçek hayat örnekleri:**
- Bilgisayar konfigürasyon sistemi — CPU, RAM, GPU, OS seçimi
- SQL sorgu builder — SELECT, WHERE, JOIN, ORDER BY zincirleme
- HTTP istek builder — header, body, auth, timeout ayarları
- Pizza siparişi — hamur, sos, malzeme seçimi

---

---

## Kötü Kullanım — Telescoping Constructor

10 parametreli constructor — okunaksız ve hataya açık:

```csharp
// Hangi bool ne anlama geliyor?
var computer = new Computer_Bad(
    "Intel i9",   // cpu
    32,            // ramGB
    1000,          // storageGB
    "RTX 4090",   // gpu
    "Windows 11", // os
    true,          // hasWifi       ← hangi bool?
    false,         // hasBluetooth  ← hangi bool?
    "Liquid",      // coolingSystem
    "850W",        // powerSupply
    "ASUS ROG"    // motherboard
);
```

### Sonuçlar:

| Sorun | Açıklama |
|---|---|
| Okunaksızlık | `true, false` — hangi bool neyi temsil ediyor? |
| Hata riski | Sıra değişirse compiler hata vermez, yanlış çalışır |
| Değişime kapalı | Yeni parametre = tüm çağrıları güncellemek |
| Opsiyonel parametre | Tüm parametreler zorunlu hale geliyor |

---

## Doğru Kullanım — Builder Pattern

Method chaining ile okunabilir, güvenli nesne oluşturma:

```csharp
// Builder sözleşmesi
public interface IComputerBuilder
{
    IComputerBuilder SetCPU(string cpu);
    IComputerBuilder SetRAM(int ramGB);
    IComputerBuilder SetGPU(string gpu);
    // ...
    Computer Build();
}

// Method chaining — her adım ne yaptığı açık!
var computer = new GamingComputerBuilder()
    .SetCPU("Intel Core i9-13900K")
    .SetRAM(32)
    .SetStorage(2000)
    .SetGPU("NVIDIA RTX 4090")
    .SetOS("Windows 11 Pro")
    .SetWifi(true)
    .SetBluetooth(true)
    .SetCoolingSystem("Liquid Cooling 360mm")
    .SetPowerSupply("1000W 80+ Gold")
    .SetMotherboard("ASUS ROG Maximus Z790")
    .Build();
```

---

## Director Rolü

Director, belirli konfigürasyonlar için inşa adımlarını standart hale getirir:

```csharp
public class ComputerDirector
{
    private IComputerBuilder _builder;

    public ComputerDirector(IComputerBuilder builder)
        => _builder = builder;

    // Standart gaming konfigürasyonu — client detayları bilmek zorunda değil
    public Computer BuildGamingComputer()
        => _builder
            .SetCPU("Intel Core i9-13900K")
            .SetRAM(32)
            .SetGPU("NVIDIA RTX 4090")
            // ...
            .Build();
}

// Kullanım — tek satır!
var director = new ComputerDirector(new GamingComputerBuilder());
var gaming   = director.BuildGamingComputer();

// Builder değiştir, aynı director farklı sonuç üretir
director.ChangeBuilder(new ServerComputerBuilder());
var server = director.BuildServerComputer();
```

---

## 🔑 Farkın Özeti

| | Telescoping Constructor | Builder Pattern |
|---|---|---|
| Okunabilirlik | ❌ `true, false` — ne anlama geliyor? | ✅ `.SetWifi(true)` — açık ve net |
| Hata riski | ❌ Sıra değişirse sessiz hata | ✅ İsimli metodlar — sıra önemsiz |
| Yeni parametre | ❌ Tüm çağrıları güncelle | ✅ Sadece builder'a ekle |
| Opsiyonel değer | ❌ Tüm parametreler zorunlu | ✅ Varsayılan değerler desteklenir |
| Standart config | ❌ Parametreler her yerde tekrar | ✅ Director ile tek yerden |

---

## Testler


### Kapsanan Senaryolar

**GamingComputerBuilderTests:**
- `Build()` → doğru CPU, RAM, Wifi değerleri
- `SetCPU()` → builder instance döner (method chaining)
- `Build()` sonrası builder reset edilir
- Boş CPU → `ArgumentException`
- Negatif RAM → `ArgumentOutOfRangeException`
- Sıfır storage → `ArgumentOutOfRangeException`

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Singleton | Creational | ✅ Tamamlandı |
| 2 | Factory Method | Creational | ✅ Tamamlandı |
| 3 | Abstract Factory | Creational | ✅ Tamamlandı |
| 4 | Prototype | Creational | ✅ Tamamlandı |
| 5 | Builder | Creational | ✅ Tamamlandı |

---
