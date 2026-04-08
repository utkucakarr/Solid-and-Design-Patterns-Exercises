using DIP_Implementation.Interfaces;
using DIP_Implementation.Services;
using DIP_Violation;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║      DIP İHLALİ — CANLI DEMO          ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var badManager = new DIP_Violation.NotificationManager();

Console.WriteLine("--- Tüm Kanallara Gönderiliyor ---");
badManager.SendAll("Siparişiniz kargoya verildi.");

Console.WriteLine();
Console.WriteLine("  Yeni bir kanal (WhatsApp) eklemek için:");
Console.WriteLine("   -> WhatsAppService sınıfı oluşturulur");
Console.WriteLine("   -> NotificationManager içine private field eklenir");
Console.WriteLine("   -> SendAll() metoduna yeni satır eklenir");
Console.WriteLine("   -> Çalışan koda dokunmak zorunda kalıyoruz!\n");

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║      DIP ÇÖZÜMÜ — DOĞRU YAKLAŞIM      ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

// Bağımlılıklar dışarıdan veriliyor — constructor injection
var services = new List<INotificationService>
{
    new EmailNotificationService(),
    new SmsNotificationService(),
    new PushNotificationService()
};

var manager = new DIP_Implementation.Services.NotificationManager(services);

Console.WriteLine("--- Tüm Kanallara Gönderiliyor (Güvenli) ---\n");
var results = manager.SendAll("Siparişiniz kargoya verildi.");

Console.WriteLine();
foreach (var result in results)
    Console.WriteLine(result.IsSuccess ? $" True: {result.Message}" : $"  False: {result.Message}");

Console.WriteLine();
Console.WriteLine("--- Belirli Kanala Gönderiliyor ---\n");
var smsResult = manager.SendToChannel("SMS", "Kargo takip numaranız: 123456");
Console.WriteLine(smsResult!.IsSuccess ? $"  True {smsResult.Message}" : $"  False: {smsResult.Message}");

Console.WriteLine();
Console.WriteLine("  WhatsApp kanalı eklemek için:");
Console.WriteLine("   -> Sadece WhatsAppNotificationService sınıfı oluşturulur");
Console.WriteLine("   -> INotificationService listesine eklenir");
Console.WriteLine("   -> NotificationManager'a hiç dokunulmaz!\n");

Console.WriteLine("--- SONUÇ ---");
Console.WriteLine("  Bad  -> Yeni kanal = mevcut kodu değiştir");
Console.WriteLine("  Good -> Yeni kanal = sadece yeni sınıf ekle\n");