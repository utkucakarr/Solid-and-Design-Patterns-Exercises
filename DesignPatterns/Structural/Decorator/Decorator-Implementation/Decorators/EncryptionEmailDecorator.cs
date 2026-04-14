using Decorator_Implementation.Interfaces;
using Decorator_Implementation.Models;

namespace Decorator_Implementation.Decorators
{
    // ConcreteDecorator — şifreleme katmanı ekliyor
    public class EncryptionEmailDecorator : BaseEmailDecorator
    {
        public EncryptionEmailDecorator(IEmailService innerService) : base(innerService)
        {
        }

        public override EmailResult Send(EmailMessage emailMessage)
        {
            ArgumentNullException.ThrowIfNull(emailMessage, nameof(emailMessage));

            // Gönderimden önce şifrele
            Console.WriteLine($"[Şifreleme] Body AES-256 ile şifreleniyor.");
            string encryptedBody = Encrypt(emailMessage.Body);

            // Yeni body ile yepyeni bir nesne yarat!
            var newEmailMessage = new EmailMessage(
                emailMessage.To,
                emailMessage.Subject,
                encryptedBody
            );

            // ✅ Inner servise ilet — zincir devam ediyor
            var result = _innerService.Send(newEmailMessage);

            return EnrichResult(result, "Encryption");
        }

        private static string Encrypt(string body)
            => Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(body)); // Gerçekte AES kullanılır

        private static EmailResult EnrichResult(EmailResult result, string decoratorName)
        {
            if (!result.IsSuccess) return result;

            var decorators = new List<string>(result.AppliedDecoratores) { decoratorName };
            return EmailResult.Success(result.To, result.Subject, decorators);
        }
    }
}
