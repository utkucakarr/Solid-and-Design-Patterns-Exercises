# Design Patterns #12 — Bridge Pattern

> *"Bir soyutlamayı implementasyonundan ayırarak ikisinin birbirinden bağımsız değişmesine olanak tanır."*
> — Gang of Four (GoF)

---

## Bridge Pattern Nedir?

Bridge Pattern, bir sınıfı iki bağımsız hiyerarşiye ayıran yapısal bir tasarım desenidir: **Abstraction** (ne yapıldığı) ve **Implementation** (nasıl yapıldığı). Her iki hiyerarşi birbirinden bağımsız olarak büyüyebilir — yeni bir abstraction eklemek implementation'ları etkilemez, yeni bir implementation eklemek abstraction'ları etkilemez.

**Ne zaman kullanılır?**

- İki boyutlu büyüme riski varsa (N×M class explosion)
- Abstraction ve implementation'ın bağımsız değişmesi gerekiyorsa
- Çalışma zamanında implementation'ı değiştirmek gerekiyorsa
- Kalıtım yerine kompozisyon tercih edilecekse

**Gerçek hayat örnekleri:**
- Rapor sistemi — rapor tipi × format (PDF, Excel, CSV)
- UI framework — widget tipi × platform (Windows, macOS, Linux)
- Veritabanı sürücüleri — sorgu tipi × DB (MySQL, PostgreSQL, SQLite)
- Mesajlaşma — mesaj tipi × kanal (Email, SMS, Push)

---

## Senaryo
Rapor Oluşturma Sistemi. Satış, Stok ve Finans raporları var. Her rapor PDF, Excel veya CSV formatında üretilebiliyor. Bridge olmadan her kombinasyon için ayrı sınıf yazılıyor — 3 rapor × 3 format = 9 sınıf. Yeni format eklenince tüm rapor sınıflarına dokunmak gerekiyor.

---

## Kötü Kullanım — Bridge İhlali

Her rapor tipi × format kombinasyonu için ayrı sınıf:

```csharp
// 3 rapor × 3 format = 9 sınıf!
public class SalesPdfReport_Bad   { public string Generate() { ... } }
public class SalesExcelReport_Bad { public string Generate() { ... } }
public class SalesCsvReport_Bad   { public string Generate() { ... } }
public class StockPdfReport_Bad   { public string Generate() { ... } }
// ... 5 sınıf daha
public class FinanceCsvReport_Bad { public string Generate() { ... } }

// PDF render kodu her sınıfta tekrar — DRY ihlali
// Yeni format (HTML) = 3 yeni sınıf, tüm raporlara dokun
// Yeni rapor (Müşteri) = 3 yeni sınıf, tüm formatlara dokun
// 4 rapor × 4 format = 16 sınıf — class explosion!
```

### Sonuçlar:

| Sorun | Açıklama |
|---|---|
| Class explosion | N rapor × M format = N×M sınıf |
| DRY ihlali | PDF render kodu her rapor tipinde tekrar |
| OCP ihlali | Yeni format = tüm rapor tiplerine dokun |
| Bağımlılık | Rapor tipi ve format birbirinden ayrılamıyor |

---

## Doğru Kullanım — Bridge Pattern

İki hiyerarşi bağımsız olarak büyüyor:

```csharp
// Implementor — sadece format biliyor
public interface IReportRenderer
{
    string RendererName { get; }
    string Render(string title, string content, Dictionary<string, string> metadata);
}

// Abstract base — Bridge referansını tutuyor
public abstract class BaseReport : IReport
{
    // Bridge — Abstraction, Implementation'ı bu referans üzerinden kullanıyor
    protected readonly IReportRenderer Renderer;

    protected BaseReport(IReportRenderer renderer)
        => Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
}

// RefinedAbstraction — sadece satış iş mantığı biliyor, format bilmiyor
public class SalesReport : BaseReport
{
    public override ReportResult Generate()
    {
        var content  = $"Toplam Satış: {_totalSales:N0} TL";
        var metadata = new Dictionary<string, string> { ... };

        // Bridge üzerinden renderer'a delege et
        return Render(content, metadata);
    }
}

// Client — runtime'da istediği kombinasyonu seçiyor
IReport report = new SalesReport(new PdfReportRenderer());
IReport report = new SalesReport(new ExcelReportRenderer()); // Sadece renderer değişti!
IReport report = new StockReport(new PdfReportRenderer());   // Sadece rapor değişti!
```

---

## Bridge'in İki Hiyerarşisi

**Abstraction Hiyerarşisi** — Ne rapor edildiği:

| Sınıf | Sorumluluk |
|---|---|
| `BaseReport` | Bridge referansını tutar, ortak render helper |
| `SalesReport` | Satış verisi toplar ve metadata üretir |
| `StockReport` | Stok verisi toplar ve sağlık durumu hesaplar |
| `FinanceReport` | Gelir/gider hesaplar, kâr marjı üretir |

**Implementation Hiyerarşisi** — Nasıl render edildiği:

| Sınıf | Sorumluluk |
|---|---|
| `PdfReportRenderer` | PDF formatında render eder |
| `ExcelReportRenderer` | Excel formatında render eder |
| `CsvReportRenderer` | CSV formatında render eder |

---

## Farkın Özeti

| | Bridge İhlali | Bridge Uyumlu |
|---|---|---|
| Sınıf sayısı | N×M kombinasyon | N + M sınıf |
| Yeni format | Tüm rapor tiplerine dokun | Yeni renderer sınıfı ekle |
| Yeni rapor | Tüm formatlara dokun | Yeni report sınıfı ekle |
| Runtime değişim | İmkansız | Renderer runtime'da değiştirilebilir |
| DRY | Format kodu her sınıfta | Renderer'da bir kez |

---

## Testler

Testler **xUnit**, **FluentAssertions** ve **Moq** ile yazılmıştır.

### Test Stratejisi

Her rapor tipi **izole** test edildi — renderer mock'landı. `BridgeIndependenceTests` ile iki hiyerarşinin gerçekten bağımsız çalıştığı doğrulandı: aynı rapor farklı renderer'larla, farklı raporlar aynı renderer ile test edildi.

### Kapsanan Senaryolar

- Her rapor tipi başarılı sonuç döner
- Doğru `ReportName` dönüyor
- Renderer'a doğru parametrelerle delege ediyor
- Constructor null ve negatif değer guard'ları
- `IReport` interface'ini implement ediyor
- **Bağımsızlık garantisi** — 9 kombinasyonun tamamı çalışıyor

---

## SOLID ile Bağlantısı

| SOLID Prensibi | Bağlantı |
|---|---|
| SRP | `SalesReport` veri toplar, `PdfReportRenderer` render eder — her sınıf tek sorumluluk |
| OCP | Yeni format = yeni renderer sınıfı; yeni rapor = yeni report sınıfı — mevcut kod değişmez |
| LSP | Tüm renderer'lar `IReportRenderer` yerine geçebilir; tüm raporlar `IReport` yerine geçebilir |
| DIP | `BaseReport` somut renderer'a değil `IReportRenderer` interface'ine bağımlı |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Amaç |
|---|---|---|
| .NET | 8.0 | Ana platform |
| xUnit | 2.9.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |
| Moq | 4.20.72 | Mock framework |

---

## 📚 Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Flyweight | Structural | ✅ Tamamlandı |
| 2 | Adapter | Structural | ✅ Tamamlandı |
| 3 | Composite | Structural | ✅ Tamamlandı |
| 4 | Facade | Structural | ✅ Tamamlandı |
| 5 | Proxy | Structural | ✅ Tamamlandı |
| 6 | Decorator | Structural | ✅ Tamamlandı |
| 7 | Bridge | Structural | ✅ Tamamlandı |

---