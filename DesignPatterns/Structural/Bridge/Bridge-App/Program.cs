using Bridge_Implementation.Interfaces;
using Bridge_Implementation.Renderers;
using Bridge_Implementation.Report;
using Bridge_Violation;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   BRIDGE İHLALİ — CANLI DEMO          ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

Console.WriteLine("--- Her kombinasyon için ayrı sınıf ---\n");
new SalesPdfReport_Bad().Generate();
new SalesExcelReport_Bad().Generate();
new StockCsvReport_Bad().Generate();
new FinancePdfReport_Bad().Generate();

Console.WriteLine();
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> 3 rapor × 3 format = 9 sınıf — class explosion");
Console.WriteLine("  -> PDF render kodu her rapor tipinde tekrar — DRY ihlali");
Console.WriteLine("  -> Yeni format (HTML) = 3 yeni sınıf, tüm raporlara dokun");
Console.WriteLine("  -> Yeni rapor (Müşteri) = 3 yeni sınıf, tüm formatlara dokun\n");

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   BRIDGE ÇÖZÜMÜ — DOĞRU YAKLAŞIM     ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

// Renderer'lar — Implementation hiyerarşisi
IReportRenderer pdf = new PdfReportRenderer();
IReportRenderer excel = new ExcelReportRenderer();
IReportRenderer csv = new CsvReportRenderer();

Console.WriteLine("--- Satış Raporu — 3 farklı format ---\n");

var salesPdf = new SalesReport(pdf);
var salesExcel = new SalesReport(excel);
var salesCsv = new SalesReport(csv);

foreach (var report in new[] { salesPdf, salesExcel, salesCsv })
{
    var result = report.Generate();
    Console.WriteLine(result.IsSuccess
        ? $" Success: {result.Message}"
        : $" Fail: {result.Message}");
}

Console.WriteLine();
Console.WriteLine("--- Tüm Raporlar — PDF formatında ---\n");

var reports = new IReport[]
{
    new SalesReport(pdf),
    new StockReport(pdf),
    new FinanceReport(pdf)
};

foreach (var report in reports)
{
    var result = report.Generate();
    Console.WriteLine(result.IsSuccess
        ? $" Success: {result.Message}"
        : $" Fail: {result.Message}");
}

Console.WriteLine();
Console.WriteLine("  HTML format eklemek istesek:");
Console.WriteLine("  -> HtmlReportRenderer : IReportRenderer — sadece bu eklenir");
Console.WriteLine("  -> SalesReport, StockReport, FinanceReport değişmez!\n");

Console.WriteLine("  Müşteri Raporu eklemek istesek:");
Console.WriteLine("  -> CustomerReport : BaseReport — sadece bu eklenir");
Console.WriteLine("  -> PDF, Excel, CSV renderer'lar değişmez!\n");

Console.WriteLine("--- SONUÇ ---");
Console.WriteLine("  Bad  -> 9 kombinasyon sınıfı — class explosion");
Console.WriteLine("  Good -> 3 rapor + 3 renderer = sonsuz kombinasyon\n");
