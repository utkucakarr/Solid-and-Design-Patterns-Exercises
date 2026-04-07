using Adapter_Implementation.Adapters;
using Adapter_Implementation.Interfaces;
using Adapter_Implementation.ThirdParty;
using Adapter_Violation.Payment;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   ADAPTER İHLALİ — CANLI DEMO         ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var badService = new CheckoutServiceBad();

Console.WriteLine("--- Ödemeler İşleniyor (Her sağlayıcı için ayrı kod) ---\n");
badService.ProcessPayment("paypall", 1000);
badService.ProcessPayment("stripe", 2500);
badService.ProcessPayment("iyzico", 500);

Console.WriteLine();
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> Her sağlayıcı için ayrı if-else — OCP ihlali");
Console.WriteLine("  -> Client her sağlayıcının API detaylarını biliyor");
Console.WriteLine("  -> Yeni sağlayıcı = CheckoutService_Bad'e dokunmak\n");

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   ADAPTER ÇÖZÜMÜ — DOĞRU YAKLAŞIM    ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

// Adapter'lar üçüncü parti servisleri sarmıyor
var processors = new List<IPaymentProcessor>
{
    new PaypalAdapter(new PayPalService()),
    new StripeAdapter(new StripeService()),
    new IyzicoAdapter(new IyzicoService())
};

Console.WriteLine("--- Tüm Ödemeler Aynı Interface ile ---\n");

foreach (var processor in processors)
{
    var result = processor.ProcessPayment(1000, "TRY");
    Console.WriteLine(result.IsSuccess
        ? $" Success: {result.Message}"
        : $" Fail: {result.Message}");
}

Console.WriteLine();
Console.WriteLine("--- İade İşlemleri ---\n");

var paypalResult = processors[0].ProcessPayment(500, "TRY");
var refundResult = processors[0].Refund(paypalResult.TransactionId, 500);
Console.WriteLine(refundResult.IsSuccess
    ? $"  Success: {refundResult.Message}"
    : $"  Fail: {refundResult.Message}");

Console.WriteLine();
Console.WriteLine("  Yeni sağlayıcı (Papara) eklemek istesek:");
Console.WriteLine("  -> PaparaService (üçüncü parti) — değiştirilmez");
Console.WriteLine("  -> PaparaAdapter : IPaymentProcessor — sadece bu eklenir");
Console.WriteLine("  -> CheckoutService'e hiç dokunulmaz!\n");

Console.WriteLine("--- SONUÇ ---");
Console.WriteLine("  Bad  -> Her sağlayıcı için ayrı kod — OCP ihlali");
Console.WriteLine("  Good -> Adapter ile tek interface — OCP korunuyor\n");