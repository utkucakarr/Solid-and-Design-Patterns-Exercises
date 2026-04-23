using Microsoft.Extensions.DependencyInjection;
using State_Implementation.Extensions;
using State_Implementation.Interfaces;
using State_Implementation.Models;
using State_Violation;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("╔══════════════════════════════════════════════════════╗");
Console.WriteLine("║   VIOLATION: if/switch Zinciri ile State Yönetimi    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════╝\n");

var badOrder = new OrderServiceBad("ORD-001", 1500m);
Console.WriteLine($"Durum: {badOrder.GetStatusDescription()}");
Console.WriteLine(badOrder.Confirm());
Console.WriteLine(badOrder.Ship());
Console.WriteLine(badOrder.Deliver());
Console.WriteLine(badOrder.Cancel()); // Teslim sonrası iptal — kural gömülü

Console.WriteLine();

//  Yeni state eklemek istesen (örn: ReturnRequested)
// Confirm(), Ship(), Deliver(), Cancel(), GetStatusDescription()
// metodlarının HEPSİNE if bloğu eklemek zorundasın!
Console.WriteLine(" Yeni 'ReturnRequested' state'i eklemek için:");
Console.WriteLine("  -> Confirm() metoduna if bloğu ekle");
Console.WriteLine("  -> Ship() metoduna if bloğu ekle");
Console.WriteLine("  -> Deliver() metoduna if bloğu ekle");
Console.WriteLine("  -> Cancel() metoduna if bloğu ekle");
Console.WriteLine("  -> GetStatusDescription() metoduna case ekle");
Console.WriteLine("  Toplam: 5 ayrı metotta değişiklik!\n");

Console.WriteLine("╔══════════════════════════════════════════════════════╗");
Console.WriteLine("║    IMPLEMENTATION: State Pattern ile Yönetim         ║");
Console.WriteLine("╚══════════════════════════════════════════════════════╝\n");

var services = new ServiceCollection();

services.AddOrder();

var provider = services.BuildServiceProvider();

var factory = provider.GetRequiredService<IOrderFactory>();

// --- Senaryo 1: Mutlu Yol (Pending → Confirmed → Shipped → Delivered) ---
Console.WriteLine(" Senaryo 1: Tam Yaşam Döngüsü");
Console.WriteLine("--------------------------------");

var order1 = factory.Create("ORD-100", 2500m);
PrintResult(order1.Confirm());
PrintResult(order1.Ship());
PrintResult(order1.Deliver());
PrintResult(order1.Cancel()); // Teslim sonrası iptal girişimi

Console.WriteLine();

// --- Senaryo 2: Pending → Cancelled ---
Console.WriteLine(" Senaryo 2: Beklemedeyken İptal");
Console.WriteLine("-----------------------------------");

var order2 = factory.Create("ORD-101", 750m);
PrintResult(order2.Confirm());
PrintResult(order2.Cancel()); // Onaylandıktan sonra iptal
PrintResult(order2.Ship());   // İptal sonrası kargo girişimi

Console.WriteLine();

// --- Senaryo 3: Geçersiz Geçişler ---
Console.WriteLine(" Senaryo 3: Geçersiz Geçiş Denemeleri");
Console.WriteLine("-------------------------------------");

var order3 = factory.Create("ORD-102", 300m);
PrintResult(order3.Ship());    // Onaysız kargo
PrintResult(order3.Deliver()); // Onaysız teslim
PrintResult(order3.Confirm());
PrintResult(order3.Ship());
PrintResult(order3.Cancel());  // Kargodayken iptal

Console.WriteLine();

// Yeni state ekleme kolaylığı
Console.WriteLine(" Yeni 'ReturnRequested' state'i eklemek için:");
Console.WriteLine("   -> Sadece ReturnRequestedState.cs dosyası oluştur");
Console.WriteLine("   -> IOrderState interface'ini implemente et");
Console.WriteLine("   -> Mevcut hiçbir state sınıfı değişmez!\n");

static void PrintResult(OrderResult result)
{
    var icon = result.IsSuccess ? "Success" : "Fail";
    var transition = result.IsSuccess
        ? $"[{result.FromState} -> {result.ToState}]"
        : $"[{result.FromState}]";
    Console.WriteLine($"  {icon} {transition} {result.Message}");
}
