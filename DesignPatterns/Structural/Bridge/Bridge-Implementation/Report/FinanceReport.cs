using Bridge_Implementation.Interfaces;
using Bridge_Implementation.Models;

namespace Bridge_Implementation.Report
{
    // RefinedAbstraction — sadece finans verisi biliyor, format bilmiyor
    public class FinanceReport : BaseReport
    {
        private readonly decimal _income;
        private readonly decimal _expense;

        public override string ReportName => "Finans Raporu";

        public FinanceReport(IReportRenderer renderer,
                             decimal income = 500_000m,
                             decimal expense = 320_000m)
            : base(renderer)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(income, nameof(income));
            ArgumentOutOfRangeException.ThrowIfNegative(expense, nameof(expense));

            _income = income;
            _expense = expense;
        }

        public override ReportResult Generate()
        {
            // Sadece finans iş mantığı — format bilmiyor
            var profit = _income - _expense;
            var content = $"Gelir: {_income:N0} TL | Gider: {_expense:N0} TL | Kâr: {profit:N0} TL";
            var metadata = new Dictionary<string, string>
            {
                ["Gelir"] = $"{_income:N0} TL",
                ["Gider"] = $"{_expense:N0} TL",
                ["Kâr"] = $"{profit:N0} TL",
                ["Kâr Marjı"] = $"{(profit / _income * 100):F1}%"
            };

            return Render(content, metadata);
        }
    }
}
