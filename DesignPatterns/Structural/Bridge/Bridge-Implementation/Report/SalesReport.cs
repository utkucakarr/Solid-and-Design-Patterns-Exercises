using Bridge_Implementation.Interfaces;
using Bridge_Implementation.Models;

namespace Bridge_Implementation.Report
{
    // RefinedAbstraction — sadece satış verisi biliyor, format bilmiyor
    public class SalesReport : BaseReport
    {
        private readonly decimal _totalSales;
        private readonly int _totalOrders;

        public override string ReportName => "Satış Raporu";

        public SalesReport(IReportRenderer renderer,
                           decimal totalSales = 150_000m,
                           int totalOrders = 320)
            : base(renderer)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(totalSales, nameof(totalSales));
            ArgumentOutOfRangeException.ThrowIfNegative(totalOrders, nameof(totalOrders));

            _totalSales = totalSales;
            _totalOrders = totalOrders;
        }

        public override ReportResult Generate()
        {
            // Sadece satış iş mantığı — format bilmiyor
            var content = $"Toplam Satış: {_totalSales:N0} TL | Sipariş: {_totalOrders} adet";
            var metadata = new Dictionary<string, string>
            {
                ["Toplam Satış"] = $"{_totalSales:N0} TL",
                ["Sipariş Adedi"] = _totalOrders.ToString(),
                ["Ortalama Sipariş"] = $"{(_totalSales / _totalOrders):N0} TL"
            };

            // Bridge üzerinden renderer'a delege et
            return Render(content, metadata);
        }
    }
}
