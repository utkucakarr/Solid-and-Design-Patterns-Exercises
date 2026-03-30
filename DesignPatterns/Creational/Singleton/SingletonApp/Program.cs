using Singleton_Implementation.Logging;
using Singleton_Violation.Logging;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║    SİNGLETON İHLALİ — CANLI DEMO     ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

Console.WriteLine("─── Her Servis Kendi Logger'ını Oluşturuyor ───\n");

var badOrderService = new OrderService();
var badUserService = new UserService();
var badPaymentService = new PaymentService();

badOrderService.CreateOrder("Laptop");
badUserService.RegisterUser("Utku");
badPaymentService.ProcessPayment(25000);

Console.WriteLine();
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> 3 farklı Logger instance oluşturuldu");
Console.WriteLine("  -> 3 ayrı nesne aynı dosyaya yazıyor — çakışma riski!");
Console.WriteLine("  -> Her instance farklı ID taşıyor — tutarsızlık\n");

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   SİNGLETON ÇÖZÜMÜ — DOĞRU YAKLAŞIM  ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

// ✅ Tek instance — tüm servisler paylaşıyor
var logger = AppLogger.GetInstance();

var orderService = new Singleton_Implementation.Services.OrderService(logger);
var userService = new Singleton_Implementation.Services.UserService(logger);
var paymentService = new Singleton_Implementation.Services.PaymentService(logger);

Console.WriteLine("─── Tüm Servisler Aynı Logger'ı Kullanıyor ───\n");

userService.RegisterUser("Utku");
orderService.CreateOrder("Laptop", 25000);
paymentService.ProcessPayment(25000, "Kredi Kartı");
userService.LoginFailed("hacker123");
orderService.CancelOrder(42);
paymentService.PaymentFailed(1000, "Yetersiz bakiye");

Console.WriteLine();

// Farklı yerlerden GetInstance() — hepsi aynı nesne!
var logger2 = AppLogger.GetInstance();
var logger3 = AppLogger.GetInstance();

Console.WriteLine($"\n  logger  ID: {logger.InstanceId}");
Console.WriteLine($"  logger2 ID: {logger2.InstanceId}");
Console.WriteLine($"  logger3 ID: {logger3.InstanceId}");
Console.WriteLine();
Console.WriteLine(logger.InstanceId == logger2.InstanceId
    ? "  Hepsi aynı instance! Singleton çalışıyor."
    : "  Farklı instance'lar!");

Console.WriteLine("\n─── SONUÇ ───");
Console.WriteLine("  Bad  -> Her servis ayrı Logger -> dosya çakışması riski");
Console.WriteLine("  Good -> Tek Logger -> thread-safe, tutarlı, sıralı loglar");
Console.WriteLine($"\n  Log dosyası: Logs/app-{DateTime.Now:yyyy-MM-dd}.log\n");