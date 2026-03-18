using OCP_Violation.Enums;
using OCP_Violation.Services;

Console.WriteLine("=== SOLID: Open/Closed Principle (OCP) Test Merkezi ===\n");

Console.WriteLine("--- 1. Senaryo: OCP İhlal Edilen Yapı ---");

var badManager = new DiscountManager();
decimal amount = 1000m;

// Enum üzerinden hesaplama yapıyoruz
var badResult = badManager.CalculateDiscount(amount, CustomerType.VIP);

Console.WriteLine($"Tutar: {amount} TL");
Console.WriteLine($"VIP İndirimi (Violation): {badResult} TL");
Console.WriteLine("Not: Yeni bir indirim eklemek için 'DiscountManager' sınıfının içine girmek zorundayız!\n");