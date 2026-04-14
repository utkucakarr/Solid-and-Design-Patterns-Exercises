using Decorator_Implementation.Interfaces;
using Decorator_Implementation.Models;

namespace Decorator_Implementation.Services
{
    // ConcreteComponent — sadece temel gönderim işlemi
    // Sıkıştırma, şifreleme, loglama bilmiyor
    public class EmailService : IEmailService
    {
        public EmailResult Send(EmailMessage emailMessage)
        {
            ArgumentNullException.ThrowIfNull(emailMessage, nameof(emailMessage));

            Console.WriteLine($"[Email] Gönderiliyor -> To: {emailMessage.To} | Subject: {emailMessage.Subject}");
            return EmailResult.Success(emailMessage.To, emailMessage.Subject, new List<string> { "Base" });
        }
    }
}
