using SRP_Violation;
using static SRP_Violation.Order;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║       SRP İHLALİ — CANLI DEMO        ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var badOrder = new Order
{
    Id = 1,
    CustomerEmail = "musteri@example.com",
    Items = new List<OrderItem>
    {
        new() { Name = "Laptop", Price = 25000, Quantity = 1 },
        new() { Name = "Mouse",  Price = 500,   Quantity = 2 }
    }
};

Console.WriteLine("--- Tek Sınıf Her Şeyi Yapıyor ---");
Console.WriteLine($"Toplam: {badOrder.CalculateTotal()} TL");
badOrder.SaveToDatabase();
badOrder.SendConfirmationEmail();
badOrder.LogOrder();

Console.WriteLine();
Console.WriteLine("  Order sınıfının değişmesi için 4 farklı sebep var:");
Console.WriteLine("  -> Veritabanı değişirse");
Console.WriteLine("  -> E-posta servisi değişirse");
Console.WriteLine("  -> Log formatı değişirse");
Console.WriteLine("  -> Hesaplama mantığı değişirse");
Console.WriteLine("   Hepsinde bu sınıfa dokunmak zorunda kalırız!\n");