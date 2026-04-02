using Prototype_Implementation.Documents;
using Prototype_Violation.Documents;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   PROTOTYPE İHLALİ — CANLI DEMO       ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var badService = new DocumentServiceBad();
badService.CreateMothlyReport("Ocak");
badService.CreateMothlyReport("Şubat");
badService.CreateWeeklyReport("1. Hafta");

Console.WriteLine();
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> Her belge sıfırdan oluşturuluyor — ağır işlem tekrarlanıyor");
Console.WriteLine("  -> Şablon parametreler her yerde kopyalanıyor — kod tekrarı");
Console.WriteLine("  -> TableData değişince her metodu güncellemek gerekiyor\n");


Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   PROTOTYPE ÇÖZÜMÜ — DOĞRU YAKLAŞIM   ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

// Şablonlar bir kez oluşturuluyor
Console.WriteLine("--- Şablonlar Oluşturuluyor (Bir Kez) ---\n");

var reportTemplate = new ReportDocument(
    title: "Aylık Rapor Şablonu",
    content: "Standart rapor içeriği...",
    tableData: new List<string> { "Gelir", "Gider", "Net Kar", "KDV" },
    metadata: new Prototype_Implementation.Models.DocumentMetadata("Sistem", "Muhasebe", "v1.0")
    );

var invoiceTemplate = new InvoiceDocument(
    title: "Fatura Şablonu",
    customerName: "Şablon Müşteri",
    items: new List<string> { "Ürün 1", "Ürün 2", "KDV"},
    totalAmount: 0,
    metadata: new Prototype_Implementation.Models.DocumentMetadata("Sistem", "Satış", "v1.0")

    );

// Registry'ye kaydet
var registry = new DocumentRegistery();
registry.Register("monthly-report", reportTemplate);
registry.Register("invoice", invoiceTemplate);

Console.WriteLine();

// Şablondan kopyala — sıfırdan oluşturma yok!
Console.WriteLine("--- Şablondan Kopyalanıyor ---\n");

var janReport = registry.CloneReport("monthly-report");
janReport.Title = "Ocak Aylık Raporu";

var febReport = registry.CloneReport("monthly-report");
febReport.Title = "Şubat Aylık Raporu";

var invoice1 = registry.CloneInvoice("invoice");
invoice1.CustomerName = "ABC Şirketi";
invoice1.TotalAmount = 15000;

Console.WriteLine();
Console.WriteLine("--- Sığ vs Derin Kopya Farkı ---\n");

var original = reportTemplate;
var shallow = reportTemplate.Clone();
var deep = reportTemplate.DeepClone();

// Sığ kopya — TableData paylaşılıyor
shallow.TableData.Add("Yeni Sütun");
Console.WriteLine($"Orijinal TableData sayısı: {original.TableData.Count}");  // 5 — etkilendi!
Console.WriteLine($"Sığ kopya TableData sayısı: {shallow.TableData.Count}");  // 5

// Derin kopya — TableData bağımsız
deep.TableData.Add("Yeni Sütun 2");
Console.WriteLine($"Orijinal TableData sayısı: {original.TableData.Count}");  // 5 — etkilenmedi!
Console.WriteLine($"Derin kopya TableData sayısı: {deep.TableData.Count}");   // 5

Console.WriteLine();
Console.WriteLine("--- Sonuçlar ---\n");
Console.WriteLine(janReport);
Console.WriteLine(febReport);
Console.WriteLine(invoice1);

Console.WriteLine("\n--- SONUÇ ---");
Console.WriteLine("  Bad  -> Her belge sıfırdan — ağır işlem tekrarı, kod kopyalanıyor");
Console.WriteLine("  Good -> Şablondan kopyala — hızlı, tutarlı, bakımı kolay\n");
