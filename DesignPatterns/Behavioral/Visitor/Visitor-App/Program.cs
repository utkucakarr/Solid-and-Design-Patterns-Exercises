using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Visitor_Implementation.Extensions;
using Visitor_Implementation.Products;
using Visitor_Implementation.Visitors;
using Visitor_Violation;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║           VIOLATION — if-else Tip Kontrolü               ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

var badService = new CatalogServiceBad();

Console.WriteLine("--- Vergi Hesaplama ---");
Console.WriteLine($"Fiziksel (1000₺, 5kg) : {badService.CalculateTax("Physical", 1000m, 5m):C}");
Console.WriteLine($"Dijital  (500₺)       : {badService.CalculateTax("Digital", 500m):C}");
Console.WriteLine($"Abonelik (199₺)       : {badService.CalculateTax("Subscription", 199m):C}");

Console.WriteLine("\n--- İndirim Hesaplama (Premium Müşteri) ---");
Console.WriteLine($"Fiziksel (1000₺) : {badService.CalculateDiscount("Physical", 1000m, true):C}");
Console.WriteLine($"Dijital  (500₺)  : {badService.CalculateDiscount("Digital", 500m, true):C}");
Console.WriteLine($"Abonelik (199₺)  : {badService.CalculateDiscount("Subscription", 199m, true):C}");

Console.WriteLine("\n--- Rapor ---");
Console.WriteLine(badService.GenerateReport("Physical", "Laptop", 1000m));
Console.WriteLine(badService.GenerateReport("Digital", "Adobe CC", 500m));
Console.WriteLine(badService.GenerateReport("Subscription", "Netflix", 199m));

Console.WriteLine();
Console.WriteLine(" Sorunlar:");
Console.WriteLine("  Yeni ürün tipi (GiftProduct) eklenince 3 metot birden güncellenir");
Console.WriteLine("  Yeni operasyon (ShippingCost) eklenince bu dev sınıf tekrar açılır");
Console.WriteLine("  string ile tip kontrolü — derleme zamanında hata yakalanmaz");
Console.WriteLine("  Her operasyon aynı sınıfta — Single Responsibility ihlali");

Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║            IMPLEMENTATION — Visitor Pattern                ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

// DI Kurulumu
var services = new ServiceCollection();

services.AddProcutVisitor();

var provider = services.BuildServiceProvider();

// Ürün kataloğu
var catalog = new List<Visitor_Implementation.Interfaces.IProduct>
{
    new PhysicalProduct("Laptop",          1000m, 2.5m),
    new PhysicalProduct("Monitör",          750m, 5.0m),
    new DigitalProduct ("Adobe CC",         500m, "https://adobe.com/download"),
    new DigitalProduct ("JetBrains IDE",    299m, "https://jetbrains.com/download"),
    new SubscriptionProduct("Netflix",      199m, 12),
    new SubscriptionProduct("Spotify",       59m, 6),
};

// Vergi Hesaplama
Console.WriteLine("--- Vergi Hesaplama (TaxCalculatorVisitor) ---");
var taxVisitor = provider.GetRequiredService<TaxCalculatorVisitor>();
decimal totalTax = 0m;
foreach (var product in catalog)
{
    var result = product.Accept(taxVisitor);
    Console.WriteLine($"  {result.Message}");
    totalTax += result.Amount ?? 0m;
}
Console.WriteLine($"  ----------------------------------------");
Console.WriteLine($"  Toplam Vergi: {totalTax:C}\n");

// İndirim Hesaplama — Premium Müşteri
Console.WriteLine("--- İndirim Hesaplama — Premium Müşteri (DiscountVisitor) ---");
var premiumDiscount = new DiscountVisitor(isPremiumCustomer: true);
decimal totalDiscount = 0m;
foreach (var product in catalog)
{
    var result = product.Accept(premiumDiscount);
    Console.WriteLine($"  {result.Message}");
    totalDiscount += result.Amount ?? 0m;
}
Console.WriteLine($"  ----------------------------------------");
Console.WriteLine($"  Toplam İndirim: {totalDiscount:C}\n");

// İndirim Hesaplama — Standart Müşteri
Console.WriteLine("--- İndirim Hesaplama — Standart Müşteri (DiscountVisitor) ---");
var standardDiscount = new DiscountVisitor(isPremiumCustomer: false);
foreach (var product in catalog)
{
    var result = product.Accept(standardDiscount);
    Console.WriteLine($"  {result.Message}");
}

//  Rapor Üretimi
Console.WriteLine("\n--- Katalog Raporu (ReportVisitor) ---");
var reportVisitor = provider.GetRequiredService<ReportVisitor>();
foreach (var product in catalog)
{
    var result = product.Accept(reportVisitor);
    Console.WriteLine($"  {result.ReportLine}");
}

Console.WriteLine("\n Yeni operasyon eklemek için sadece yeni bir Visitor sınıfı yaz!");
Console.WriteLine("   Örnek: ShippingCostVisitor — hiçbir ürün sınıfı değişmez.");