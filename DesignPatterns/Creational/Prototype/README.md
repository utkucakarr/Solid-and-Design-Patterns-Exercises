# Design Patterns #4 — Prototype Pattern

> *"Oluşturulacak nesnenin türünü, prototip bir instance kullanarak belirler ve bu prototipi kopyalayarak yeni nesneler üretir."*
> — Gang of Four (GoF)

---

## Prototype Pattern Nedir?

Prototype Pattern, mevcut bir nesneyi **kopyalayarak** yeni nesneler oluşturur. Nesneyi sıfırdan oluşturmak yerine, var olan bir şablondan türetilir. Bu sayede hem nesne oluşturma maliyeti düşer hem de şablon tutarlılığı sağlanır.

**Ne zaman kullanılır?**

- Nesne oluşturmanın maliyetli olduğu durumlarda (DB sorgusu, dosya okuma, ağır hesaplama)
- Benzer nesnelerin tekrar tekrar oluşturulması gerektiğinde
- Şablon mantığının merkezi tutulması gerektiğinde
- Nesnenin iç yapısı dışarıya açılmak istenmediğinde

**Gerçek hayat örnekleri:**
- Belge yönetim sistemi — rapor, fatura, sözleşme şablonları
- Oyun geliştirme — düşman, silah, harita nesneleri
- Grafik editör — şekil, katman kopyalama
- Konfigürasyon yönetimi — varsayılan ayarlardan türetme

---

---

## Kötü Kullanım — Prototype İhlali

Her belge sıfırdan oluşturuluyor — şablon mantığı yok:

```csharp
public class DocumentService_Bad
{
    public ReportDocument_Bad CreateMonthlyReport(string month)
    {
        // Ağır işlem her seferinde tekrarlanıyor
        return new ReportDocument_Bad(
            title:     $"{month} Aylık Raporu",
            content:   "Standart rapor içeriği...",  // tekrar eden şablon
            tableData: new List<string>              // her seferinde aynı liste
            {
                "Gelir", "Gider", "Net Kâr", "KDV"
            },
            author: "Sistem"
        );
    }

    public ReportDocument_Bad CreateWeeklyReport(string week)
    {
        // Aynı şablon parametreler tekrar yazılıyor!
        return new ReportDocument_Bad(
            title:     $"{week} Haftalık Raporu",
            content:   "Standart rapor içeriği...",
            tableData: new List<string>
            {
                "Gelir", "Gider", "Net Kâr", "KDV"
            },
            author: "Sistem"
        );
    }
}
```

### Sonuçlar:

| Sorun | Açıklama |
|---|---|
| Performans | Her belge için ağır işlem tekrarlanıyor |
| Kod tekrarı | Şablon parametreler her yerde kopyalanıyor |
| Bakım zorluğu | Şablon değişince tüm metodlara dokunmak gerekiyor |
| Tutarsızlık | Her metod farklı parametreler geçirebilir |

---

## Doğru Kullanım — Prototype Pattern

Şablon bir kez oluşturuluyor, kopyalanarak kullanılıyor:

```csharp
// Sözleşme — her belge kendini kopyalayabilir
public interface IDocumentPrototype<T> where T : class
{
    T Clone();      // Sığ kopya
    T DeepClone();  // Derin kopya
}

// Şablon bir kez oluşturuluyor
var reportTemplate = new ReportDocument(
    title:     "Aylık Rapor Şablonu",
    content:   "Standart rapor içeriği...",
    tableData: new List<string> { "Gelir", "Gider", "Net Kâr", "KDV" },
    metadata:  new DocumentMetadata("Sistem", "Muhasebe", "v1.0")
);

// Registry'ye kaydet
var registry = new DocumentRegistry();
registry.Register("monthly-report", reportTemplate);

// Şablondan kopyala — sıfırdan oluşturma yok!
var janReport = registry.CloneReport("monthly-report");
janReport.Title = "Ocak Aylık Raporu";

var febReport = registry.CloneReport("monthly-report");
febReport.Title = "Şubat Aylık Raporu";
```

---

## Sığ Kopya vs Derin Kopya

Prototype Pattern'in en kritik konusu:

```csharp
var original = reportTemplate;
var shallow  = reportTemplate.Clone();      // Sığ kopya
var deep     = reportTemplate.DeepClone();  // Derin kopya

// Sığ kopya — referans tipler paylaşılıyor
shallow.TableData.Add("Yeni Sütun");
Console.WriteLine(original.TableData.Count); // 5 — ETKİLENDİ!

// Derin kopya — tüm referans tipler bağımsız
deep.TableData.Add("Yeni Sütun");
Console.WriteLine(original.TableData.Count); // 4 — ETKİLENMEDİ!
```

| | Sığ Kopya (Clone) | Derin Kopya (DeepClone) |
|---|---|---|
| Value types | Kopyalanır | Kopyalanır |
| String | Kopyalanır | Kopyalanır |
| Reference types | Aynı referans | Yeni nesne |
| Liste/Koleksiyon | Paylaşılır | Yeni liste |
| Ne zaman? | Bağımsız değişiklik gerekmiyorsa | Tam bağımsızlık gerekiyorsa |

---

## Farkın Özeti

| | Prototype İhlali | Prototype Uyumlu |
|---|---|---|
| Nesne oluşturma | Her seferinde sıfırdan | Şablondan kopyala |
| Performans | Ağır işlem tekrarı | Bir kez oluştur, kopyala |
| Kod tekrarı | Şablon parametreler kopyalanıyor | Merkezi şablon yönetimi |
| Şablon değişikliği | Tüm metodlara dokunmak lazım | Registry'de tek değişiklik |
| Tutarlılık | Her metod farklı parametre geçebilir | Şablondan türetilmiş garantisi |

---

## Testler

Testler **xUnit** ve **FluentAssertions** ile yazılmıştır.

### Kapsanan Senaryolar

**ReportDocumentTests:**
- `Clone()` → yeni instance döner, title kopyalanır
- `Clone()` → TableData aynı referansı paylaşır (sığ kopya davranışı)
- `Clone()` sonrası TableData değiştirilince orijinal etkilenir
- `DeepClone()` → yeni instance, bağımsız TableData ve Metadata
- `DeepClone()` sonrası değişiklik orijinali etkilemez
- Guard clause testleri

**InvoiceDocumentTests:**
- `DeepClone()` tüm özellikleri kopyalar
- CustomerName değişikliği orijinali etkilemez
- Items listesi bağımsız kopyalanır

**DocumentRegistryTests:**
- `Register()` → count artar
- `CloneReport()` → yeni bağımsız instance döner
- Bilinmeyen key → `KeyNotFoundException`
- Birden fazla kopyalama bağımsız nesneler üretir

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Singleton | Creational | ✅ Tamamlandı |
| 2 | Factory Method | Creational | ✅ Tamamlandı |
| 3 | Abstract Factory | Creational | ✅ Tamamlandı |
| 4 | Prototype | Creational | ✅ Tamamlandı |
| 5 | Builder | Creational | 🔜 Yakında |

---

Her türlü soru ve geri bildirim için LinkedIn üzerinden ulaşabilirsiniz.
