using Microsoft.Extensions.DependencyInjection;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using TemplateMethod_Implementation.Extensions;
using TemplateMethod_Implementation.Interfaces;
using TemplateMethod_Implementation.Reports;
using TemplateMethod_Violation;

// Ekrana türkçe karakterleri düzgün yazdırmak için
Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("╔══════════════════════════════════════════════╗");
Console.WriteLine("║       VIOLATION — Kopya Yapıştır Yaklaşım    ║");
Console.WriteLine("╚══════════════════════════════════════════════╝");
Console.WriteLine();

var sampleData = new[] { "Ocak: 12.000₺", "Şubat: 15.500₺", "Mart: 9.800₺" };

var pdfBad = new PdfReportGeneratorBad();
var excelBad = new ExcelReportGeneratorBad();
var csvBad = new CsvReportGeneratorBad();

Console.WriteLine("--- PDF (Bad) ---");
Console.WriteLine(pdfBad.Generate("Q1 Satış Raporu", sampleData));

Console.WriteLine("--- Excel (Bad) ---");
Console.WriteLine(excelBad.Generate("Q1 Satış Raporu", sampleData));

Console.WriteLine("--- CSV (Bad) ---");
Console.WriteLine(csvBad.Generate("Q1 Satış Raporu", sampleData));

Console.WriteLine();
Console.WriteLine(" Sorunlar:");
Console.WriteLine(" - Veri doğrulama 3 sınıfta ayrı ayrı yazıldı");
Console.WriteLine(" - CSV sınıfında LOG atılmayı geliştirici unuttu");
Console.WriteLine(" - Yeni adım (audit log) eklemek için 3 dosya değiştirilmeli");
Console.WriteLine(" - Algoritma sırası sınıflar arasında farklılaşabilir");

Console.WriteLine();
Console.WriteLine("-----------------------------------------");
Console.WriteLine();

Console.WriteLine("╔══════════════════════════════════════════════╗");
Console.WriteLine("║    IMPLEMENTATION — Template Method          ║");
Console.WriteLine("╚══════════════════════════════════════════════╝");
Console.WriteLine();

var services = new ServiceCollection();
services.AddReportGenerator();
var provider = services.BuildServiceProvider();

var generators = new IReportGenerator[]
{
    provider.GetRequiredService<PdfReportGenerator>(),
    provider.GetRequiredService<ExcelReportGenerator>(),
    provider.GetRequiredService<CsvReportGenerator>()
};

foreach (var generator in generators)
{
    var result = generator.Generate("Q1 Satış Raporu", sampleData);

    Console.WriteLine($"--- {result.Format} ---");
    if (result.IsSuccess)
    {
        Console.WriteLine(result.Content);
        Console.WriteLine($" {result.Message}");
        Console.WriteLine($" Oluşturulma: {result.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC");
    }
    else
    {
        Console.WriteLine($" Hata: {result.Message}");
    }
    Console.WriteLine();
}

Console.WriteLine(" Yeni format eklemek için sadece ReportGeneratorBase'i miras alan");
Console.WriteLine(" yeni bir sınıf yazılır — mevcut hiçbir kod değişmez.");
Console.WriteLine();
Console.WriteLine(" Log formatını değiştirmek için sadece base class'taki");
Console.WriteLine(" Log() metodu güncellenir — tüm sınıflar otomatik güncellenir.");