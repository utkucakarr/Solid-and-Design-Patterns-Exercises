using SRP_Implementation.Logging;
using SRP_Implementation.Models;
using SRP_Implementation.Repositories;
using SRP_Implementation.Services;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║     SRP ÇÖZÜMÜ — DOĞRU YAKLAŞIM      ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var order = new Order
{
    Id = 2,
    CustomerEmail = "musteri@example.com",
    Items = new List<OrderItem>
    {
        new() { Name = "Laptop", Price = 25000, Quantity = 1 },
        new() { Name = "Mouse",  Price = 500,   Quantity = 2 }
    }
};

Console.WriteLine("--- Her Sınıf Tek Bir İş Yapıyor ---");
Console.WriteLine($"Toplam: {order.CalculateTotal()} TL\n");

var service = new OrderService(
    new OrderRepository(),
    new OrderNotificationService(),
    new OrderLogger()
);

service.ProcessOrder(order);

Console.WriteLine("\n--- SONUÇ ---");
Console.WriteLine("  Bad -> 1 sınıf, 4 sorumluluk. Bir şey değişince her yere dokunuyorsun.");
Console.WriteLine("  Good -> Her sınıfın tek işi var. Değişim izole kalıyor.\n");