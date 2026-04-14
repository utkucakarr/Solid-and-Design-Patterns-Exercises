namespace Decorator_Implementation.Models
{
    public class EmailResult
    {
        public bool IsSuccess { get; }
        public string To { get; }
        public string Subject { get; }
        public string Message { get; }
        public IReadOnlyList<string> AppliedDecoratores { get; }

        private EmailResult(
            bool isSuccess,
            string to,
            string subject,
            string message,
            List<string> appliedDecorators)
        {
            IsSuccess = isSuccess;
            To = to;
            Subject = subject;
            Message = message;
            AppliedDecoratores = appliedDecorators;
        }

        public static EmailResult Success(
            string to, 
            string subject,
            List<string> appliedDecorators) 
            => new(true, to, subject, 
                $"E-posta başarıyla gönderildi. To: {to} | " + 
                $"Katmanlar: {string.Join(" -> ", appliedDecorators)}", 
                appliedDecorators);

        public static EmailResult Fail(string to, string reason)
            => new(false, to, string.Empty, 
                $"E-posta gönderilemedi. To: {to} | Hata: {reason}", 
                new List<string>());
    }
}
