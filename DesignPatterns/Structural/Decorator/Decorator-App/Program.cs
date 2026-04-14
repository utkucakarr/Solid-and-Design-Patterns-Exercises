using Decorator_Implementation.Decorators;
using Decorator_Implementation.Interfaces;
using Decorator_Implementation.Models;
using Decorator_Implementation.Services;
using static Decorator_Violation.Email.EmailServiceBad;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   DECORATOR İHLALİ — CANLI DEMO       ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var message_Bad = new EmailMessageBad
{
    To = "utku@test.com",
    Subject = "Test",
    Body = "Merhaba!"
};

Console.WriteLine("--- Her kombinasyon için ayrı sınıf ---\n");
new BasicEmailServiceBad().Send(message_Bad);
new CompressedEmailServiceBad().Send(message_Bad);
new CompressedEncryptedLoggedEmailService_Bad().Send(message_Bad);

Console.WriteLine();
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> 3 özellik = 8 farklı sınıf kombinasyonu — class explosion");
Console.WriteLine("  -> Gönderim kodu her sınıfta tekrar yazıldı — DRY ihlali");
Console.WriteLine("  -> Yeni özellik (retry) = tüm kombinasyonlara dokun\n");


Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   DECORATOR ÇÖZÜMÜ — DOĞRU YAKLAŞIM  ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var message = new EmailMessage
(
    "utku@test.com",
    "Sipariş Onayı",
    "Siparişiniz onaylandı."
);

// Seçenek 1 — Sadece temel gönderim
Console.WriteLine("--- Seçenek 1: Sadece temel gönderim ---\n");
IEmailService basic = new EmailService();
var result1 = basic.Send(message);
Console.WriteLine(result1.IsSuccess ? $" Success: {result1.Message}\n" : $" Fail: {result1.Message}\n");

// Seçenek 2 — Sıkıştırma + Şifreleme
Console.WriteLine("--- Seçenek 2: Sıkıştırma + Şifreleme ---\n");
IEmailService compressed_encrypted = new EncryptionEmailDecorator(
                                     new CompressionEmailDecorator(
                                     new EmailService()));
var result2 = compressed_encrypted.Send(message);
Console.WriteLine(result2.IsSuccess ? $" Success: {result2.Message}\n" : $" Fail: {result2.Message}\n");

// Seçenek 3 — Tüm katmanlar zincirde
Console.WriteLine("─── Seçenek 3: Loglama + Şifreleme + Sıkıştırma ───\n");
IEmailService fullChain = new LoggingEmailDecorator(
                          new EncryptionEmailDecorator(
                          new CompressionEmailDecorator(
                          new EmailService())));
var result3 = fullChain.Send(message);
Console.WriteLine(result3.IsSuccess ? $" Success: {result3.Message}\n" : $" Fail: {result3.Message}\n");

Console.WriteLine("  Retry decorator eklemek istesek:");
Console.WriteLine("  -> RetryEmailDecorator : BaseEmailDecorator — sadece bu eklenir");
Console.WriteLine("  -> Mevcut hiçbir sınıf değişmez!\n");

Console.WriteLine("--- SONUÇ ---");
Console.WriteLine("  Bad  -> 8 kombinasyon sınıfı — class explosion");
Console.WriteLine("  Good -> 3 decorator + zincirleme — sonsuz kombinasyon\n");