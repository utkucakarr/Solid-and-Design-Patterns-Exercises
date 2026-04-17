# Design Patterns #6 — Flyweight Pattern

> *"Çok sayıda benzer nesneyi verimli şekilde desteklemek için paylaşımı kullanan yapısal bir tasarım desenidir."*
> — Gang of Four (GoF)

---

## Flywieght Pattern Nedir?

Flyweight Pattern, çok sayıda benzer nesnenin bellek kullanımını azaltmak için **ortak verileri paylaşır**. Nesnelerin değişmeyen ortak verileri (intrinsic) tek bir yerde tutulur, nesneye özel veriler (extrinsic) ise dışarıdan sağlanır.

**Ne zaman kullanılır?**

- Çok sayıda benzer nesne oluşturulacaksa ve bellek kritikse
- Nesnelerin büyük kısmı aynı veriyi tekrar tutuyor ise
- Extrinsic ve intrinsic veriler birbirinden ayrılabiliyorsa
- Uygulama performansı bellek kullanımına bağlıysa

**Gerçek hayat örnekleri:**
- Orman simülasyonu — milyonlarca ağaç, az sayıda tip
- Metin editörü — her karakter nesnesi yerine karakter tipi paylaşımı
- Oyun geliştirme — mermi, düşman, parçacık sistemleri
- Harita uygulamaları — tekrar eden arazi tipleri

---

## Senaryo
Bir oyunda devasa bir orman render ediliyor:

1.000.000 ağaç var
Her ağacın türü, dokusu ve rengi var — bunlar paylaşılabilir (intrinsic)
Her ağacın konumu ve boyutu var — bunlar her nesneye özel (extrinsic)

Eğer 1 milyon ağacın hepsini ayrı nesne olarak saklarsak → bellek çöker!
Flyweight ile → aynı türdeki ağaçlar tek bir nesneyi paylaşır → bellek %99 azalır!

---

## Kötü Kullanım — Flyweight İhlali

Her ağaç kendi texture ve color verisini tutuyor:

```csharp
public class Tree_Bad
{
    // Intrinsic veriler — her nesnede tekrar ediliyor
    public string TreeType { get; }  // "Oak"
    public string Texture { get; }   // MB boyutunda texture!
    public string Color { get; }     // Renk bilgisi

    // Extrinsic veriler
    public int X { get; }
    public int Y { get; }
    public int Size { get; }
}
```

### Sonuçlar:

| Senaryo | Bellek Kullanımı |
|---|---|
| 1.000 ağaç | 1.000 texture kopyası |
| 100.000 ağaç | 100.000 texture kopyası |
| 1.000.000 ağaç | Bellek çöküyor! |

---

## Doğru Kullanım — Flyweight Pattern

Intrinsic ve extrinsic veriler ayrılıyor:

```csharp
// Flyweight — paylaşılan intrinsic veriler
public sealed class TreeType
{
    public string Name { get; }    // "Oak"
    public string Color { get; }   // "Koyu Yeşil"
    public string Texture { get; } // "oak_texture"

    // Extrinsic veriler dışarıdan geliyor
    public void Render(int x, int y, int size) { ... }
}

// Tree — sadece extrinsic veriler
public class Tree
{
    public int X { get; }
    public int Y { get; }
    public int Size { get; }
    private readonly TreeType _treeType; // Paylaşılan referans!
}
```

### Sonuçlar:

| Senaryo | TreeType Nesnesi | Tree Nesnesi |
|---|---|---|
| 1.000.000 Oak ağacı | **1 adet** | 1.000.000 adet |
| 1.000.000 karışık ağaç (3 tip) | **3 adet** | 1.000.000 adet |

---

## Intrinsic vs Extrinsic

Flyweight'in temeli bu ayrımdır:

| | Intrinsic State | Extrinsic State |
|---|---|---|
| Tanım | Değişmeyen, paylaşılabilir veri | Nesneye özel, değişen veri |
| Nerede tutulur? | Flyweight nesnesinde | Dışarıda (client veya context) |
| Örnek | Ağaç tipi, texture, renk | X koordinatı, Y koordinatı, boyut |
| Bellek | Paylaşıldığı için az | Her nesne için ayrı |

---

## Farkın Özeti

| | Flyweight İhlali | Flyweight Uyumlu |
|---|---|---|
| Bellek kullanımı | O(n) — ağaç sayısıyla orantılı | O(k) — benzersiz tip sayısıyla |
| 1M ağaç belleği | 1M texture kopyası | Sadece 3 TreeType nesnesi |
| Paylaşım | Her nesne kendi verisini taşır | Ortak veriler paylaşılır |
| Performans | Düşük — bellek baskısı | Yüksek — az bellek kullanımı |

---

## Testler

### Kapsanan Senaryolar

**TreeFactoryTests:**
- Aynı parametreler → **aynı instance** döner (Flyweight garantisi!)
- Farklı parametreler → farklı instance döner
- Aynı parametreler defalarca çağrılsa UniqueCount artmıyor
- Farklı parametreler → UniqueCount artıyor
- Cache başlangıçta sıfır

**ForestTests:**
- `PlantTree()` TreeCount artırıyor
- Aynı tipte 3 ağaç → UniqueTreeTypes = **1**
- Farklı tipte 3 ağaç → UniqueTreeTypes = **3**
- **1000 aynı tipte ağaç → hâlâ 1 TreeType!**
- 3000 karışık ağaç (3 tip) → sadece 3 TreeType
- Negatif koordinat → `ArgumentOutOfRangeException`

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Flyweight | Structural | ✅ Tamamlandı |
| 2 | Adapter | Structural | 🔜 Yakında |
| 3 | Composite | Structural | 🔜 Yakında |
| 4 | Facade | Structural | 🔜 Yakında |
| 5 | Proxy | Structural | 🔜 Yakında |
| 6 | Decorator | Structural | 🔜 Yakında |
| 7 | Bridge | Structural | 🔜 Yakında |

---
