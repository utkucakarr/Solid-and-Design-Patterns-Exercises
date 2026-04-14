using Decorator_Implementation.Interfaces;
using Decorator_Implementation.Models;

namespace Decorator_Implementation.Decorators
{
    // ConcreteDecorator - sıkıştırma katmanı ekliyor
    public class CompressionEmailDecorator : BaseEmailDecorator
    {
        public CompressionEmailDecorator(IEmailService innerService) : base(innerService)
        {
        }

        public override EmailResult Send(EmailMessage emailMessage)
        {
            ArgumentNullException.ThrowIfNull(emailMessage, nameof(emailMessage));

            // Göndermeden önce sıkıştır
            var originalSize = emailMessage.Body.Length;
            string compressedBody = Compress(emailMessage.Body);
            var compressedSize = compressedBody.Length;

            Console.WriteLine($"[Sıkıştırma] {originalSize} → {compressedSize} karakter");

            // Yepyeni bir EmailMessage üretiyoruz!
            var newEmailMessage = new EmailMessage(
                emailMessage.To,
                emailMessage.Subject,
                compressedBody // Sıkıştırılmış metni veriyoruz
            );

            // Inner servise ilet — zincir devam ediyor
            var result = _innerService.Send(newEmailMessage);

            // Sonucu bu katmanın bilgisiyle zenginleştir
            return EnrichResult(result, "Compression");
        }

        public static string Compress(string body)
            => $"[COMPRESSED]{body}[/COMPRESSED]"; // Gerçekte GZip kullanılır

        private static EmailResult EnrichResult(EmailResult result, string decoratorName)
        {
            if (!result.IsSuccess) return result;

            var decorators = new List<string>(result.AppliedDecoratores) { decoratorName };
            return EmailResult.Success(result.To, result.Subject, decorators);
        }
    }
}
