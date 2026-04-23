using Visitor_Implementation.Interfaces;
using Visitor_Implementation.Models;
using Visitor_Implementation.Products;

namespace Visitor_Implementation.Visitors
{
    // Sadece rapor üretme sorumluluğu
    public class ReportVisitor : IProductVisitor
    {
        // Fiziksel ürün raporu
        public VisitResult Visit(PhysicalProduct product)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var line = $"[FİZİKSEL] {product.Name,-25} | " +
                       $"Fiyat: {product.BasePrice,8:C} | " +
                       $"Ağırlık: {product.WeightKg:0.##} kg | " +
                       $"Kargo: Gerekli";

            return VisitResult.Report(line);
        }

        // Dijital ürün raporu
        public VisitResult Visit(DigitalProduct product)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var line = $"[DİJİTAL]  {product.Name,-25} | " +
                       $"Fiyat: {product.BasePrice,8:C} | " +
                       $"URL: {product.DownloadUrl} | " +
                       $"Anında Teslimat";

            return VisitResult.Report(line);
        }

        // Abonelik ürünü raporu
        public VisitResult Visit(SubscriptionProduct product)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var totalPrice = product.BasePrice * product.DurationMonths;
            var line = $"[ABONELİK] {product.Name,-25} | " +
                       $"Aylık: {product.BasePrice,8:C} | " +
                       $"Süre: {product.DurationMonths} ay | " +
                       $"Toplam: {totalPrice:C}";

            return VisitResult.Report(line);
        }
    }
}
