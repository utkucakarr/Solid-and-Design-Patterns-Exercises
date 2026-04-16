# SOLID #4 — Interface Segregation Principle (ISP)

> *"İstemciler, kullanmadıkları metodlara bağımlı olmaya zorlanmamalıdır."*
> — Robert C. Martin

---

## Prensip Nedir?

Interface Segregation Principle, büyük ve şişirilmiş interface'ler yerine **küçük ve odaklı interface'ler** tasarlanması gerektiğini söyler.

Bir sınıf bir interface'i implemente etmek zorunda kalıyorsa ama o interface'teki bazı metodları desteklemiyorsa — bu interface çok büyük demektir. Bu durumda sınıf ya boş bir implementasyon yazmak ya da `NotSupportedException` fırlatmak zorunda kalır. Her ikisi de ISP ihlalidir.

**Test sorusu:** Bir sınıf implemente ettiği interface'deki her metodu gerçekten destekliyor mu?

- Evet → ISP uyumlu
- Hayır / Exception fırlatıyor → ISP ihlali

---

## Kötü Kullanım — ISP İhlali

`IPrinter_Bad` interface'i tüm yetenekleri tek çatı altında topluyor:

```csharp
public interface IPrinter_Bad
{
    void Print(string document);
    void Scan(string document);  // Her yazıcı tarayabilir mi? HAYIR!
    void Fax(string document);   // Her yazıcı faks gönderebilir mi? HAYIR!
}
```

Bu interface'i implemente etmek zorunda kalan `BasicPrinter_Bad` çaresiz kalıyor:

```csharp
public class BasicPrinter_Bad : IPrinter_Bad
{
    public void Print(string document) => Console.WriteLine("Yazdırıldı.");

    // Desteklemediği metodları exception fırlatarak geçiştiriyor
    public void Scan(string document) => throw new NotSupportedException("Tarama yapamaz!");
    public void Fax(string document)  => throw new NotSupportedException("Faks gönderemez!");
}
```

### Sonuç: Runtime'da patlıyor!

```csharp
var printers = new List<IPrinter_Bad>
{
    new AllInOnePrinter_Bad(),
    new OfficePrinter_Bad(),
    new BasicPrinter_Bad()
};

foreach (var printer in printers)
    printer.Scan("Rapor.pdf");
```

---

## Doğru Kullanım — ISP Uyumlu Yapı

Her yetenek ayrı ve küçük bir interface ile temsil ediliyor:

```csharp
public interface IPrintable { void Print(string document); }
public interface IScannable { void Scan(string document); }
public interface IFaxable   { void Fax(string document); }
```

Her cihaz **yalnızca desteklediği interface'leri** implemente ediyor:

| Cihaz | IPrintable | IScannable | IFaxable |
|---|---|---|---|
| `BasicPrinter` | Destekler | Desteklemez | Desteklemez |
| `OfficePrinter` | Destekler | Destekler | Desteklemez |
| `AllInOnePrinter` | Destekler | Destekler | Destekler |

```csharp
// Tüm cihazlar yazdırabilir — güvenli!
var allPrintables = new List<IPrintable> { basicPrinter, officePrinter, allInOne };

// Sadece tarayabilenler — BasicPrinter giremez, compiler engeller!
var scannables = new List<IScannable> { officePrinter, allInOne };

// Sadece faks gönderebilenler
var faxables = new List<IFaxable> { allInOne };
```

## Testler

Testler **xUnit** ve **FluentAssertions** ile yazılmıştır.

### Kapsanan Senaryolar

**BasicPrinterTests:**
- Yazdırma işlemi başarılı sonuç döndürüyor.
- Boş belge adıyla `ArgumentException` fırlatılıyor.
- `IPrintable` atanabilir, `IScannable` ve `IFaxable` **atanamıyor.**

**OfficePrinterTests:**
- Yazdırma ve tarama işlemleri başarılı sonuç döndürüyor.
- `IPrintable` ve `IScannable` atanabilir, `IFaxable` **atanamıyor.**

**AllInOnePrinterTests:**
- Yazdırma, tarama ve faks işlemleri başarılı sonuç döndürüyor.
- `IPrintable`, `IScannable` ve `IFaxable` hepsi atanabilir.

---

## LSP ile Bağlantısı

ISP ve LSP birbirini tamamlar. ISP ihlali çoğunlukla LSP ihlaline yol açar:

> Interface çok büyük → Sınıf bazı metodları destekleyemiyor → `NotSupportedException` fırlatıyor → LSP ihlali

Interface'leri küçük tutarak hem ISP'yi hem de LSP'yi aynı anda koruyabilirsin.
