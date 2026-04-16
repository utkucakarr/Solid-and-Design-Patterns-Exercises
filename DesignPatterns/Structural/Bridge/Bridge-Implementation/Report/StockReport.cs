using Bridge_Implementation.Interfaces;
using Bridge_Implementation.Models;

namespace Bridge_Implementation.Report
{
    // RefinedAbstraction — sadece stok verisi biliyor, format bilmiyor
    public class StockReport : BaseReport
    {
        private readonly int _totalProducts;
        private readonly int _criticalStock;

        public override string ReportName => "Stok Raporu";

        public StockReport(IReportRenderer renderer,
                           int totalProducts = 5_420,
                           int criticalStock = 12)
            : base(renderer)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(totalProducts, nameof(totalProducts));
            ArgumentOutOfRangeException.ThrowIfNegative(criticalStock, nameof(criticalStock));

            _totalProducts = totalProducts;
            _criticalStock = criticalStock;
        }

        public override ReportResult Generate()
        {
            // Sadece stok iş mantığı — format bilmiyor
            var content = $"Toplam Ürün: {_totalProducts} | Kritik Stok: {_criticalStock}";
            var metadata = new Dictionary<string, string>
            {
                ["Toplam Ürün"] = _totalProducts.ToString(),
                ["Kritik Stok"] = _criticalStock.ToString(),
                ["Stok Sağlığı"] = _criticalStock < 20 ? "Kritik" : "Normal"
            };

            return Render(content, metadata);
        }
    }
}
