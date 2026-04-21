using Microsoft.Extensions.DependencyInjection;
using Observer_Implementation.Extensions;
using Observer_Implementation.Observers;
using Observer_Implementation.Subject;
using Observer_Violation;

#region VIOLATION — if/switch cehennemi

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                VIOLATION YAKLAŞIMI                           ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine(" Tüm bildirim ve iş mantığı OrderService_Bad içine gömülmüş.");
Console.WriteLine(" Yeni bir kanal (WhatsApp) eklemek ChangeOrderStatus'u şişirir.");
Console.WriteLine(" Her durum için if/else zinciri büyümeye devam eder.");
Console.WriteLine(" Test edilebilirlik sıfır — her şey Console.WriteLine ile doğrudan.");
Console.WriteLine();

var badService = new OrderServiceBad();

var badOrder = new Observer_Violation.Order(
    orderId: "ORD-001",
    customerEmail: "ahmet@example.com",
    customerPhone: "+905551234567",
    customerDeviceToken: "device-token-abc",
    productName: "MacBook Pro",
    quantity: 1,
    totalPrice: 85000m);

Console.WriteLine(" Sipariş oluşturuluyor...");
badService.PlaceOrder(badOrder);

Console.WriteLine();
Console.WriteLine(" Sipariş durumu Confirmed yapılıyor...");
badService.ChangeOderStatus("ORD-001", Observer_Violation.OrderStatus.Confirmed);

Console.WriteLine();
Console.WriteLine(" Sipariş durumu Shipped yapılıyor...");
badService.ChangeOderStatus("ORD-001", Observer_Violation.OrderStatus.Shipped);

Console.WriteLine();
Console.WriteLine(" WhatsApp bildirimi eklemek için ChangeOrderStatus içine");
Console.WriteLine(" her if bloğuna elle kod yazmak gerekiyor. OCP ihlali!");

#endregion

Console.WriteLine();
Console.WriteLine("---");
Console.WriteLine();

#region ✅ IMPLEMENTATION — Observer Pattern ile temiz çözüm

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                IMPLEMENTATION YAKLAŞIMI                      ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// DI Container kurulumu
var services = new ServiceCollection();
services.AddOrderObserverService();
var provider = services.BuildServiceProvider();

var orderService = provider.GetRequiredService<OrderService>();
var emailObserver = provider.GetRequiredService<EmailNotificationObserver>();
var smsObserver = provider.GetRequiredService<SmsNotificationObserver>();
var pushObserver = provider.GetRequiredService<PushNotificationObserver>();
var inventoryObs = provider.GetRequiredService<InventoryObserver>();
var invoiceObs = provider.GetRequiredService<InvoiceObserver>();


// Observer'lar subscribe ediliyor
orderService.Subscribe(emailObserver);
orderService.Subscribe(smsObserver);
orderService.Subscribe(pushObserver);
orderService.Subscribe(inventoryObs);
orderService.Subscribe(invoiceObs);

var order = new Observer_Implementation.Models.Order(
    orderId: "ORD-002",
    customerEmail: "mehmet@example.com",
    customerPhone: "+905559876543",
    customerDeviceToken: "device-token-xyz",
    productName: "iPhone 15 Pro",
    quantity: 2,
    totalPrice: 120000m);

// Sipariş oluşturuluyor
Console.WriteLine(" Sipariş oluşturuluyor...");
var placeResult = orderService.PlaceOrder(order);
Console.WriteLine($" -> {placeResult.Message}");

Console.WriteLine();

// Confirmed
Console.WriteLine(" Sipariş durumu Confirmed yapılıyor...");
var confirmResult = orderService.ChangeOrderStatus("ORD-002", Observer_Implementation.Models.OrderStatus.Confirmed);
Console.WriteLine($" -> {confirmResult.Message}");

Console.WriteLine();

// Shipped
Console.WriteLine(" Sipariş durumu Shipped yapılıyor...");
var shipResult = orderService.ChangeOrderStatus("ORD-002", Observer_Implementation.Models.OrderStatus.Shipped);
Console.WriteLine($" -> {shipResult.Message}");

Console.WriteLine();

// Delivered
Console.WriteLine(" Sipariş durumu Delivered yapılıyor...");
var deliverResult = orderService.ChangeOrderStatus("ORD-002", Observer_Implementation.Models.OrderStatus.Delivered);
Console.WriteLine($" -> {deliverResult.Message}");

Console.WriteLine();
Console.WriteLine("══════════════════════════════════════════════════════════════");
Console.WriteLine();

// Unsubscribe demosu
Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║           UNSUBSCRIBE & YENİ OBSERVER DEMOsu                 ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine(" SMS Observer unsubscribe ediliyor...");
orderService.Unsubscribe(smsObserver);
Console.WriteLine(" -> SmsNotificationObserver artık bildirim almayacak.");
Console.WriteLine();

var order2 = new Observer_Implementation.Models.Order(
    orderId: "ORD-003",
    customerEmail: "ayse@example.com",
    customerPhone: "+905550001122",
    customerDeviceToken: "device-token-def",
    productName: "AirPods Pro",
    quantity: 1,
    totalPrice: 8500m);

Console.WriteLine(" Yeni sipariş oluşturuluyor (SMS olmadan)...");
var place2Result = orderService.PlaceOrder(order2);
Console.WriteLine($" -> {place2Result.Message}");

Console.WriteLine();
Console.WriteLine(" Sipariş Confirmed yapılıyor (SMS gelmeyecek)...");
var confirm2Result = orderService.ChangeOrderStatus("ORD-003", Observer_Implementation.Models.OrderStatus.Confirmed);
Console.WriteLine($" -> {confirm2Result.Message}");

Console.WriteLine();
Console.WriteLine("══════════════════════════════════════════════════════════════");
Console.WriteLine();

// İptal senaryosu — stok iadesi
Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║               İPTAL SENARYOSU — STOK İADESİ                  ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

var order3 = new Observer_Implementation.Models.Order(
    orderId: "ORD-004",
    customerEmail: "can@example.com",
    customerPhone: "+905553334455",
    customerDeviceToken: "device-token-ghi",
    productName: "iPad Pro",
    quantity: 3,
    totalPrice: 75000m);

orderService.Subscribe(smsObserver); // SMS tekrar abone

Console.WriteLine(" Sipariş oluşturuluyor...");
orderService.PlaceOrder(order3);

Console.WriteLine();
Console.WriteLine(" Sipariş Confirmed yapılıyor (stok rezerve edilecek)...");
orderService.ChangeOrderStatus("ORD-004", Observer_Implementation.Models.OrderStatus.Confirmed);

Console.WriteLine();
Console.WriteLine(" Sipariş Cancelled yapılıyor (stok iade edilecek)...");
var cancelResult = orderService.ChangeOrderStatus("ORD-004", Observer_Implementation.Models.OrderStatus.Cancelled);
Console.WriteLine($" -> {cancelResult.Message}");

Console.WriteLine();
Console.WriteLine("══════════════════════════════════════════════════════════════");
Console.WriteLine();
Console.WriteLine(" Observer Pattern ile:");
Console.WriteLine("  OrderService hangi observer'ın ne yaptığını bilmiyor");
Console.WriteLine("  Yeni kanal eklemek için sadece yeni sınıf yazılıyor");
Console.WriteLine("  Subscribe/Unsubscribe ile runtime'da esneklik sağlanıyor");
Console.WriteLine("  Her observer tek bir sorumluluğa sahip (SRP)");
Console.WriteLine("  OrderService değişmeden yeni observer eklenebiliyor (OCP)");

#endregion