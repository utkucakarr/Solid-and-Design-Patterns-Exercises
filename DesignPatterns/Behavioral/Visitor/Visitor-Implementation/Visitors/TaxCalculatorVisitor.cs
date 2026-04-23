using Visitor_Implementation.Interfaces;
using Visitor_Implementation.Models;
using Visitor_Implementation.Products;

namespace Visitor_Implementation.Visitors
{
    // Sadece vergi hesaplama sorumluluğu
    public class TaxCalculatorVisitor : IProductVisitor
    {
        // Fiziksel ürün: %18 KDV + ağırlık bazlı ek vergi (0.5₺/kg)
        public VisitResult Visit(PhysicalProduct product)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var kdv = product.BasePrice * 0.18m;
            var weightTax = product.WeightKg * 0.5m;
            var totalTax = kdv + weightTax;

            return VisitResult.Success(
                totalTax,
                $"[Vergi] {product.Name}: KDV {kdv:C} + Ağırlık Vergisi {weightTax:C} = {totalTax:C}");
        }

        // Dijital ürün: %8 KDV — indirimli oran
        public VisitResult Visit(DigitalProduct product)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var tax = product.BasePrice * 0.08m;

            return VisitResult.Success(
                tax,
                $"[Vergi] {product.Name}: KDV (Dijital) {tax:C}");
        }

        // Abonelik: %18 KDV * ay sayısı — toplam dönem vergisi
        public VisitResult Visit(SubscriptionProduct product)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var monthlyTax = product.BasePrice * 0.18m;
            var totalTax = monthlyTax * product.DurationMonths;

            return VisitResult.Success(
                totalTax,
                $"[Vergi] {product.Name}: Aylık KDV {monthlyTax:C} x {product.DurationMonths} ay = {totalTax:C}");
        }
    }
}
