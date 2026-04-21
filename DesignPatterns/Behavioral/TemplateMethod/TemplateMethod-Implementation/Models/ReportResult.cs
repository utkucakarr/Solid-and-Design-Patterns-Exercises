namespace TemplateMethod_Implementation.Models
{
    public sealed class ReportResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string? Content { get; }
        public string? Format { get; }
        public DateTime GeneratedAt { get; }

        private ReportResult(bool isSuccess, string message, string? content, string? format)
        {
            IsSuccess = isSuccess;
            Message = message;
            Content = content;
            Format = format;
            GeneratedAt = DateTime.UtcNow;
        }

        public static ReportResult Success(string content, string format, string reportTitle) =>
            new(true, $"'{reportTitle}' raporu başarıyla oluşturuldu [{format}].", content, format);

        public static ReportResult Fail(string reason) =>
            new(false, reason, null, null);
    }
}
