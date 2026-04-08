using FactoryMethod_Implementation.Factories;
using FactoryMethod_Implementation.Interfaces;
using FactoryMethod_Vıolation.Notifications;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║  FACTORY METHOD İHLALİ — CANLI DEMO   ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var badService = new NotificationServiceBad();

Console.WriteLine("--- Bildirimler Gönderiliyor ---\n");
badService.Send("email", "utku@example.com", "Siparişiniz kargoya verildi.");
badService.Send("sms", "+905001234567", "Kargo takip: 123456");
badService.Send("push", "device-token-abc", "Yeni mesajınız var.");

Console.WriteLine();
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> Yeni kanal eklemek için NotificationServiceBad içine girmek lazım");
Console.WriteLine("  -> if-else zinciri büyüdükçe bakımı zorlaşıyor");
Console.WriteLine("  -> Nesne oluşturma sorumluluğu client'a yüklenmiş\n");


Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║     FACTORY METHOD ÇÖZÜMÜ             ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var factories = new List<INotificationFactory> {
    new EmailNotificationFactory(),
    new SmsNotificationFactory(),
    new PushNotificationFactory()
};

var recipients = new[]
{
    "utku@example.com",
    "+905001234567",
    "device-token-abc"
};

var messages = new[]
{
    "Siparişiniz kargoya verildi.",
    "Kargo takip: 123456",
    "Yeni mesajınız var."
};

Console.WriteLine("--- Bildirimler Gönderiliyor (Güvenli) ---\n");

for (int i = 0; i < factories.Count; i++)
{
    var notification = factories[i].CreateNotification();
    var result = notification.Send(recipients[i], messages[i]);

    Console.WriteLine(result.IsSuccess
    ? $"  {result.Message}"
    : $"  {result.Message}");
}

Console.WriteLine();
Console.WriteLine("    WhatsApp kanalı eklemek istesek:");
Console.WriteLine(" -> WhatsAppNotification sınıfı oluşturulur");
Console.WriteLine(" -> WhatsAppNotificationFactory sınıfı oluşturulur");
Console.WriteLine(" -> NotificationService'e hiç dokunulmaz!\n");

Console.WriteLine("--- SONUÇ ---");
Console.WriteLine("  Bad  -> if-else zinciri — yeni kanal = mevcut koda dokun");
Console.WriteLine("  Good -> Factory — yeni kanal = sadece yeni sınıf ekle\n");