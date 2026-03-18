using OCP_Implementation.Services;
using OCP_Implementation.Strategies;

Console.WriteLine("--- 2. Senaryo: OCP Uygulanan Yapı ---");

var goodCalculator = new DiscountCalculator();

decimal amount = 1000m;

// Strateji nesnesini (VipDiscount) dışarıdan "enjekte" ediyoruz
var vipStrategy = new VipDiscount();
var goodResult = goodCalculator.Calculate(amount, vipStrategy);

Console.WriteLine($"Tutar: {amount} TL");
Console.WriteLine($"VIP İndirimi (Implementation): {goodResult} TL");