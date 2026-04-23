namespace Visitor_Implementation.Models
{
    public sealed class VisitResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public decimal? Amount { get; }
        public string? ReportLine { get; }

        private VisitResult(bool isSuccess, string message, decimal? amount, string? reportLine)
        {
            IsSuccess = isSuccess;
            Message = message;
            Amount = amount;
            ReportLine = reportLine;
        }

        public static VisitResult Success(decimal amount, string message) =>
            new(true, message, amount, null);

        public static VisitResult Report(string reportLine) =>
            new(true, "Rapor satırı oluşturuldu.", null, reportLine);

        public static VisitResult Fail(string reason) =>
            new(false, reason, null, null);
    }
}
