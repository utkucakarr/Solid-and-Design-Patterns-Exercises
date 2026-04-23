using ChainOfResponsibility_Implementation.Extensions;
using ChainOfResponsibility_Implementation.Handlers;
using ChainOfResponsibility_Implementation.Models;
using ChainOfResponsibility_Violation;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║          VIOLATION — Tek Metotta Her Şey                 ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

var badService = new CheckoutServiceBad();

Console.WriteLine("--- Senaryo 1: Stok yetersiz ---");
Console.WriteLine(badService.ProcessOrder("PROD-002", 5, "CUST-001", 5000m));

Console.WriteLine("\n--- Senaryo 2: Fraud tespit ---");
Console.WriteLine(badService.ProcessOrder("PROD-001", 1, "FRAUD-USER", 5000m));

Console.WriteLine("\n--- Senaryo 3: Başarılı sipariş ---");
Console.WriteLine(badService.ProcessOrder("PROD-001", 1, "CUST-001", 5000m));

Console.WriteLine();
Console.WriteLine(" Sorunlar:");
Console.WriteLine("  Yeni kontrol eklemek için dev CheckoutService_Bad sınıfını açmak gerekir");
Console.WriteLine("  Adımlar bağımsız test edilemiyor");
Console.WriteLine("  Sıra değiştirmek tüm metodu yeniden yazmayı gerektirir");
Console.WriteLine("  Fraud kontrolü atlanıp direkt ödemeye geçilemez (A/B test vb.)");


Console.WriteLine("  ╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("  ║          IMPLEMENTATION — Zincir Yaklaşımı               ║");
Console.WriteLine("  ╚══════════════════════════════════════════════════════════╝\n");

// DI Kurulumu
var services = new ServiceCollection();

services.AddOrderHander();

var provider = services.BuildServiceProvider();

// Zinciri kur — fluent API ile
var stockHandler = provider.GetRequiredService<StockHandler>();
var fraudHandler = provider.GetRequiredService<FraudHandler>();
var paymentHandler = provider.GetRequiredService<PaymentHandler>();
var shippingHandler = provider.GetRequiredService<ShippingHandler>();

stockHandler
    .SetNext(fraudHandler)
    .SetNext(paymentHandler)
    .SetNext(shippingHandler);

// Senaryo 1: Stok yetersiz
Console.WriteLine("--- Senaryo 1: Stok yetersiz (PROD-002) ---");
var req1 = new OrderRequest("PROD-002", "CUST-001", 5, 5000m);
var res1 = stockHandler.Handle(req1);
Console.WriteLine($"  Sonuç: {(res1.IsSuccess ? "Success" : "Fail")} {res1.Message}");
Console.WriteLine($"  Kırılan Handler: {res1.FailedHandler ?? "-"}\n");

// Senaryo 2: Fraud tespit
Console.WriteLine("--- Senaryo 2: Fraud tespit (CUST-FRAUD) ---");
var req2 = new OrderRequest("PROD-001", "CUST-FRAUD", 1, 5000m);
var res2 = stockHandler.Handle(req2);
Console.WriteLine($"  Sonuç: {(res2.IsSuccess ? "Success" : "Fail")} {res2.Message}");
Console.WriteLine($"  Kırılan Handler: {res2.FailedHandler ?? "-"}\n");

// Senaryo 3: Bakiye yetersiz
Console.WriteLine("--- Senaryo 3: Bakiye yetersiz ---");
var req3 = new OrderRequest("PROD-001", "CUST-001", 5, 100m);
var res3 = stockHandler.Handle(req3);
Console.WriteLine($"  Sonuç: {(res3.IsSuccess ? "Success" : "Fail")} {res3.Message}");
Console.WriteLine($"  Kırılan Handler: {res3.FailedHandler ?? "-"}\n");

// Senaryo 4: Dijital ürün — kargo yok
Console.WriteLine("--- Senaryo 4: Dijital ürün (kargo yok) ---");
var req4 = new OrderRequest("DIGITAL-ONLY", "CUST-001", 1, 5000m);
var res4 = stockHandler.Handle(req4);
Console.WriteLine($"  Sonuç: {(res4.IsSuccess ? "Success" : "Fail")} {res4.Message}");
Console.WriteLine($"  Kırılan Handler: {res4.FailedHandler ?? "-"}\n");

// Senaryo 5: Her şey başarılı
Console.WriteLine("--- Senaryo 5: Tam başarılı sipariş ---");
var req5 = new OrderRequest("PROD-001", "CUST-001", 1, 5000m);
var res5 = stockHandler.Handle(req5);
Console.WriteLine($"  Sonuç: {(res5.IsSuccess ? "Success" : "Fail")} {res5.Message}");
Console.WriteLine($"  Takip Kodu: {res5.TrackingCode ?? "-"}\n");

Console.WriteLine(" Yeni handler eklemek için sadece yeni bir sınıf yaz ve zincire ekle!");
Console.WriteLine(" Örnek: stockHandler.SetNext(loyaltyHandler).SetNext(fraudHandler)...");