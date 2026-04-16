namespace Bridge_Implementation.Models
{
    public class ReportResult
    {
        public bool IsSuccess { get; }
        public string ReportName { get; }
        public string RendererName { get; }
        public string Content { get; }
        public string Message { get; }

        private ReportResult(
            bool isSuccess,
            string reportName,
            string rendererName,
            string content,
            string message)
        {
            IsSuccess = isSuccess;
            ReportName = reportName;
            RendererName = rendererName;
            Content = content;
            Message = message;
        }

        public static ReportResult Success(
            string reportName, string rendererName, string content)
             => new(true, reportName, rendererName, content,
                 $"{reportName} raporu {rendererName} formatında başarıyla oluşturuldu.");

        public static ReportResult Fail(string reportName, string reason)
            => new(false, reportName, string.Empty, string.Empty,
                $"{reportName} raporu oluşturulamadı: {reason}");
    }
}
