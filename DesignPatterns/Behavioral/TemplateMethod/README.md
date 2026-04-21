# Template Method Pattern

> *"Bir işlemdeki algoritmanın iskeletini tanımlar ve bazı adımların uygulanmasını alt sınıflara bırakır. Template Method (Şablon Metot), alt sınıfların algoritmanın genel yapısını değiştirmeden belirli adımlarını yeniden tanımlamasına olanak tanır."*
> — **Gang of Four**

---

## Pattern Nedir?

**Template Method**, bir algoritmanın iskeletini base class'ta tanımlayan, değişen adımları ise subclass'lara bırakan bir **Behavioral (Davranışsal)** tasarım kalıbıdır.

### Ne Zaman Kullanılır?

- Birden fazla sınıf **aynı algoritma sırasını** takip ediyorsa ama adımların **uygulaması** farklıysa
- Ortak adımların (doğrulama, loglama, hata yönetimi) **tek yerden yönetilmesi** gerekiyorsa
- Kod tekrarını (DRY ihlalini) ortadan kaldırmak istiyorsak
- Algoritmanın **sırasının** subclass'lar tarafından bozulmasını engellemek istiyorsak

### Gerçek Hayat Örnekleri

| Domain | Sabit Adımlar | Değişen Adımlar |
|---|---|---|
| Rapor oluşturma | Veri getir → Doğrula → Log | Formatlama (PDF/Excel/CSV) |
| Veri migrasyonu | Bağlan → Oku → Yaz → Kapat | Dönüşüm mantığı |
| OAuth akışı | Token al → Doğrula → Yetkilendir | Provider'a göre endpoint |
| Build pipeline | Derle → Test → Paketle | Deploy hedefine göre adımlar |
| Oyun turu | Başlat → Hamle al → Bitir | Her oyunun hamle kuralları |

---

## Kötü Kullanım

Her rapor sınıfı kendi `Generate` metodunu baştan sona kendisi yazıyor:

```csharp
// PdfReportGenerator_Bad — algoritma iskeleti yok
public class PdfReportGenerator_Bad
{
    public string Generate(string reportTitle, IEnumerable<string> data)
    {
        // Veri getirme — ExcelReportGenerator_Bad'de de aynı
        var rows = data.ToList();
        if (!rows.Any())
            return "Hata: Veri bulunamadı.";

        // Doğrulama — 3 sınıfta 3 kez yazıldı
        if (string.IsNullOrWhiteSpace(reportTitle))
            return "Hata: Başlık boş olamaz.";

        // Formatlama (bu kısım farklı olmalıydı — gerisi kopyalandı)
        var sb = new StringBuilder();
        sb.AppendLine("[PDF HEADER]");
        // ...

        // Log — CsvReportGenerator_Bad'de bu satır unutuldu!
        Console.WriteLine($"[LOG] PDF raporu oluşturuldu: {reportTitle}");
        return sb.ToString();
    }
}
```

### Sonuçlar

| Sorun | Etkisi |
|---|---|
| Doğrulama 3 kez yazıldı | Kural değişince 3 dosya güncellenmeli |
| CSV'de log atılmadı | Production'da sessiz hata, izlenemiyor |
| Algoritma sırası dağınık | Yeni geliştirici yanlış sıra uygulayabilir |
| Yeni format eklemek pahalı | Her seferinde tüm iskelet yeniden yazılıyor |

---

## Doğru Kullanım

Algoritma iskeleti `ReportGeneratorBase`'de sabitlendi, değişen adımlar `abstract` olarak subclass'lara bırakıldı:

```csharp
// Template Method — sealed ile sıra korunuyor
public abstract class ReportGeneratorBase : IReportGenerator
{
    public sealed ReportResult Generate(string reportTitle, IEnumerable<string> data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reportTitle, nameof(reportTitle));
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        var rows = FetchData(data);                          // Ortak adım 1
        var error = Validate(rows);                          // Ortak adım 2
        if (error is not null) return ReportResult.Fail(error);

        var sb = new StringBuilder();
        sb.AppendLine(FormatHeader(reportTitle));            // Değişen adım
        sb.AppendLine(FormatRows(rows));                     // Değişen adım
        sb.AppendLine(FormatFooter(reportTitle, rows.Count));// Değişen adım

        Log(reportTitle);                                    // Ortak adım 3
        return ReportResult.Success(sb.ToString(), FormatName, reportTitle);
    }

    protected abstract string FormatName { get; }
    protected abstract string FormatHeader(string reportTitle);
    protected abstract string FormatRows(List<string> rows);
    protected abstract string FormatFooter(string reportTitle, int rowCount);
}

// Concrete sınıf sadece farklı adımları uygular
public sealed class PdfReportGenerator : ReportGeneratorBase
{
    protected override string FormatName => "PDF";

    protected override string FormatHeader(string reportTitle) =>
        $"[PDF HEADER]\nBaşlık: {reportTitle}";

    protected override string FormatRows(List<string> rows) =>
        string.Join("\n", rows.Select(r => $"  • {r}"));

    protected override string FormatFooter(string reportTitle, int rowCount) =>
        $"[PDF FOOTER] Toplam: {rowCount} kayıt — Gizlidir";
}
```

---

## Pattern'in Yaptığı İşlem

| Adım | Sorumlu | Açıklama |
|---|---|---|
| 1. FetchData | Base (virtual) | Veriyi listeye çevirir |
| 2. Validate | Base (virtual) | Boş veri kontrolü |
| 3. FormatHeader | Subclass (abstract) | Her format kendi başlığını yazar |
| 4. FormatRows | Subclass (abstract) | Her format satırları farklı biçimler |
| 5. FormatFooter | Subclass (abstract) | Her format kendi footer'ını yazar |
| 6. Log | Base (virtual) | Merkezi loglama, tutarlı format |
| 7. Result | Base | Immutable ReportResult döner |

---

## Farkın Özeti

| Kriter | Bad (Kopya Yapıştır) | Good (Template Method) |
|---|---|---|
| Algoritma iskeleti | Her sınıfta ayrı | Base class'ta tek, `sealed` |
| Ortak adımlar | 3 kez tekrar | Bir kez yazıldı |
| Log tutarlılığı | CSV'de unutuldu | Merkezi, hiç atlanamaz |
| Yeni format ekleme | Tüm iskelet yeniden | Sadece 3 abstract metot |
| Kural değişikliği | 3 dosya güncelleme | 1 dosya güncelleme |
| Algoritma sırası | Geliştirici bozabilir | `sealed` ile korunuyor |
| OCP uyumu | Her ekleme mevcut kodu bozar | Sadece yeni sınıf eklenir |

---

## Testler

### Neden Moq Kullanılmadı?

Template Method pattern'inde test edilen şey concrete sınıfların davranışıdır. `ReportGeneratorBase` abstract olduğu için doğrudan test edilemez; `PdfReportGenerator`, `ExcelReportGenerator` ve `CsvReportGenerator` zaten tam implementasyonlardır. Moq, yalnızca dış bağımlılıkları (veritabanı, HTTP servisi) taklit etmek için anlamlıdır — burada böyle bir bağımlılık yoktur.

### Kapsanan Senaryolar

| Kategori | Test Sayısı | Örnek |
|---|---|---|
| Happy path | 5 | PDF/Excel/CSV başarılı üretim |
| Algoritma sırası | 3 | Header → Rows → Footer sırası |
| Boş veri | 4 | `Array.Empty<string>()` → Fail |
| Guard clause | 9 | null/empty/whitespace başlık |
| FormatName doğrulama | 3 | `result.Format.Should().Be("PDF")` |
| Mesaj içeriği | 2 | Title ve format adı mesajda var |
| **Toplam** | **27** | |

---

## SOLID ile Bağlantısı

| Prensip | İlişki |
|---|---|
| **SRP** | Her concrete sınıfın tek sorumluluğu: kendi formatını üretmek |
| **OCP** | Yeni format eklemek için mevcut kod değişmez, sadece yeni subclass yazılır |
| **LSP** | Her subclass `IReportGenerator` sözleşmesini eksiksiz karşılar |
| **DIP** | `Program.cs` somut sınıflara değil `IReportGenerator` interface'ine bağımlı |

---

## Diğer Pattern'lerle İlişkisi

| Pattern | İlişki |
|---|---|
| **Strategy** | İkisi de davranışı değiştirir; Strategy runtime'da seçilir ve kompozisyon kullanır, Template Method compile-time'da kalıtımla çalışır |
| **Factory Method** | Factory Method, Template Method'un nesne yaratma adımını override eden özel bir halidir |
| **Facade (08)** | Facade karmaşık alt sistemleri tek arayüzde toplar; Template Method ise tek algoritmanın adımlarını yönetir |
| **Hook Method** | Template Method'un uzantısı: base class'ta boş virtual metotlar tanımlanır, subclass'lar isteğe bağlı override eder |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|---|---|---|
| .NET | 8.0 | Platform |
| Microsoft.Extensions.DependencyInjection | 10.0.6 | DI container |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |

---

## Design Patterns Serisi

| # | Pattern | Kategori | Domain | Durum |
|---|---|---|---|---|
| 1 | Strategy | Behavioral | ✅ Tamamlandı |
| 2 | Command | Behavioral | ✅ Tamamlandı |
| 3 | Iterator | Behavioral | ✅ Tamamlandı |
| 4 | Template Metot | Behavioral | ✅ Tamamlandı |
| 5 | Observer | Behavioral | 🔜 Yakında |
| 6 | Memento | Behavioral | 🔜 Yakında |
| 7 | Mediator | Behavioral | 🔜 Yakında |
| 8 | Chain Of Responsibility | Behavioral | 🔜 Yakında |
| 9 | Visitor | Behavioral | 🔜 Yakında |
| 10 | State | Behavioral | 🔜 Yakında |