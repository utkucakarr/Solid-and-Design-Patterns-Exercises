using Microsoft.Extensions.DependencyInjection;
using Strategy_Implementation.Extensions;
using Strategy_Implementation.Interfaces;
using Strategy_Implementation.Strategies;
using Strategy_Implementation.Models;
using Strategy_Violation;
using Strategy_Violation.Models;

Console.WriteLine("╔══════════════════════════════════════════════════════╗");
Console.WriteLine("║                İHLAL YAKLAŞIMI                    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════╝\n");

var badService = new ShippingServiceBad();

var badOrders = new[]
{
    new Strategy_Violation.Models.ShippingOrder { OrderId = "ORD-001", WeightKg = 3.5, OrderTotal = 250m, MemberShipType = "standard", ShippingType = "standard" },
    new Strategy_Violation.Models.ShippingOrder { OrderId = "ORD-002", WeightKg = 2.0, OrderTotal = 300m, MemberShipType = "standard", ShippingType = "express" },
    new Strategy_Violation.Models.ShippingOrder { OrderId = "ORD-003", WeightKg = 5.0, OrderTotal = 600m, MemberShipType = "standard", ShippingType = "free" },
    new Strategy_Violation.Models.ShippingOrder { OrderId = "ORD-004", WeightKg = 4.0, OrderTotal = 400m, MemberShipType = "premium",  ShippingType = "member" },
    new Strategy_Violation.Models.ShippingOrder { OrderId = "ORD-005", WeightKg = 1.0, OrderTotal = 200m, MemberShipType = "standard", ShippingType = "free" },
};

foreach (var order in badOrders)
{
    var result = badService.CalculateShipping(order);
    string icon = result.IsSuccess ? "Success" : "Fail";
    Console.WriteLine($"{icon} [{order.OrderId}] Tip: {order.ShippingType,-10} | " +
                      $"Sonuç: {result.Message,-45} | Ücret: {result.Cost,8}");
}

Console.WriteLine("\n  Sorunlar:");
Console.WriteLine("   - Yeni kargo tipi eklemek için ShippingService_Bad değiştirilmeli (OCP ihlali)");
Console.WriteLine("   - Algoritmalar test edilemiyor çünkü sınıftan ayrılmış değil");
Console.WriteLine("   - Kargo tipini runtime'da değiştirmek mümkün değil\n");

Console.WriteLine("╔══════════════════════════════════════════════════════╗");
Console.WriteLine("║               DOĞRU YAKLAŞIM                     ║");
Console.WriteLine("╚══════════════════════════════════════════════════════╝\n");

//  DI Container kurulumu
var services = new ServiceCollection();

services.AddShippingService();

var provider = services.BuildServiceProvider();
var context = provider.GetRequiredService<IShippingContext>();

var goodOrders = new[]
{
    (Order: new Strategy_Implementation.Models.ShippingOrder{ OrderId = "ORD-101", WeightKg = 3.5, OrderTotal = 250m, MembershipType = "standard" },
     Strategy: (IShippingStrategy)provider.GetRequiredService<StandardShippingStrategy>()),

    (Order: new Strategy_Implementation.Models.ShippingOrder { OrderId = "ORD-102", WeightKg = 2.0, OrderTotal = 300m, MembershipType = "standard" },
     Strategy: (IShippingStrategy)provider.GetRequiredService<ExpressingShippingStrategy>()),

    (Order: new Strategy_Implementation.Models.ShippingOrder { OrderId = "ORD-103", WeightKg = 5.0, OrderTotal = 600m, MembershipType = "standard" },
     Strategy: (IShippingStrategy)provider.GetRequiredService<FreeShippingStrategy>()),

    (Order: new Strategy_Implementation.Models.ShippingOrder { OrderId = "ORD-104", WeightKg = 4.0, OrderTotal = 400m, MembershipType = "premium" },
     Strategy: (IShippingStrategy)provider.GetRequiredService<MemberShippingStrategy>()),

    (Order: new Strategy_Implementation.Models.ShippingOrder { OrderId = "ORD-105", WeightKg = 1.0, OrderTotal = 200m, MembershipType = "standard" },
     Strategy: (IShippingStrategy)provider.GetRequiredService<FreeShippingStrategy>()),
};

foreach (var (order, strategy) in goodOrders)
{
    context.SetStrategy(strategy);
    var result = context.ExecuteShipping(order);
    string icon = result.IsSuccess ? "Success" : "Fail";
    Console.WriteLine($"{icon} [{order.OrderId}] Strateji: {result.StrategyUsed,-22} | " +
                      $"Sonuç: {result.Message,-45} | Ücret: {result.Cost,8}");
}

Console.WriteLine("\n Avantajlar:");
Console.WriteLine("   - Yeni kargo tipi eklemek için sadece yeni bir strateji sınıfı yeterli (OCP)");
Console.WriteLine("   - Her strateji bağımsız test edilebilir");
Console.WriteLine("   - Runtime'da strateji değiştirilebilir (örn: flash sale sırasında FreeShipping)");

//  Runtime'da strateji değişimi demo
Console.WriteLine("\n Runtime Strateji Değişimi Demo:");
var flashSaleOrder = new Strategy_Implementation.Models.ShippingOrder
{
    OrderId = "ORD-FLASH",
    WeightKg = 2.0,
    OrderTotal = 1200m,
    MembershipType = "standard"
};

context.SetStrategy(provider.GetRequiredService<StandardShippingStrategy>());
var before = context.ExecuteShipping(flashSaleOrder);
Console.WriteLine($"   Normal  -> {before.StrategyUsed}: {before.Cost}");

context.SetStrategy(provider.GetRequiredService<FreeShippingStrategy>());
var after = context.ExecuteShipping(flashSaleOrder);
Console.WriteLine($"   Flash  -> {after.StrategyUsed}: {after.Cost}");