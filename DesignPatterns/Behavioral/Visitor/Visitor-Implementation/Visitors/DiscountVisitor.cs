using Visitor_Implementation.Interfaces;
using Visitor_Implementation.Models;
using Visitor_Implementation.Products;

namespace Visitor_Implementation.Visitors
{
    // Sadece indirim hesaplama sorumluluğu
    public class DiscountVisitor : IProductVisitor
    {
        private readonly bool _isPremiumCustomer;

        public DiscountVisitor(bool isPremiumCustomer)
        {
            _isPremiumCustomer = isPremiumCustomer;
        }

        // Fiziksel ürün: Premium %15, Normal %5
        public VisitResult Visit(PhysicalProduct product)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var rate = _isPremiumCustomer ? 0.15m : 0.05m;
            var discount = product.BasePrice * rate;

            return VisitResult.Success(
                discount,
                $"[İndirim] {product.Name}: %{rate * 100:0} indirim = {discount:C}");
        }

        // Dijital ürün: Premium %20, Normal %10
        public VisitResult Visit(DigitalProduct product)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var rate = _isPremiumCustomer ? 0.20m : 0.10m;
            var discount = product.BasePrice * rate;

            return VisitResult.Success(
                discount,
                $"[İndirim] {product.Name}: %{rate * 100:0} indirim = {discount:C}");
        }

        // Abonelik: Sadece premium müşteriye %25 indirim
        public VisitResult Visit(SubscriptionProduct product)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            if (!_isPremiumCustomer)
                return VisitResult.Success(0m,
                    $"[İndirim] {product.Name}: Standart müşteri — abonelikte indirim yok.");

            var discount = product.BasePrice * 0.25m;

            return VisitResult.Success(
                discount,
                $"[İndirim] {product.Name}: Premium abonelik indirimi = {discount:C}");
        }
    }
}