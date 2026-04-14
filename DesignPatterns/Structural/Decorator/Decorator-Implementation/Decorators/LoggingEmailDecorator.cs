using Decorator_Implementation.Interfaces;
using Decorator_Implementation.Models;

namespace Decorator_Implementation.Decorators
{
    // ConcreteDecorator — loglama katmanı ekliyor
    public class LoggingEmailDecorator : BaseEmailDecorator
    {
        public LoggingEmailDecorator(IEmailService innerService) : base(innerService)
        {
        }

        public override EmailResult Send(EmailMessage emailMessage)
        {
            ArgumentNullException.ThrowIfNull(emailMessage, nameof(emailMessage));

            // Göndermeden logla
            var startTime = DateTime.UtcNow;
            Console.WriteLine($"[Log] Gönderim başlıyor. To: {emailMessage.To} | " +
                  $"Time: {startTime:HH:mm:ss.fff}");

            // Inner servise ilet — zincir devam ediyor
            var result = _innerService.Send(emailMessage);

            // Gönderimden sonra logla
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            Console.WriteLine($"[Log] Gönderim tamamlandı. " +
                              $"Başarı: {result.IsSuccess} | Süre: {duration:F1}ms");

            return EnrichResult(result, "Logging");
        }

        private static EmailResult EnrichResult(EmailResult result, string decoratorName)
        {
            if (!result.IsSuccess) return result;

            var decorators = new List<string>(result.AppliedDecoratores) { decoratorName };
            return EmailResult.Success(result.To, result.Subject, decorators);
        }
    }
}
