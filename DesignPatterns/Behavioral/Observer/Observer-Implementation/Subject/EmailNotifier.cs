using Observer_Implementation.Interfaces;

namespace Observer_Implementation.Subject
{
    public class EmailNotifier : IEmailNotifier
    {
        // E-posta gönderimi tek sorumluluğa sahip — gerçek SMTP burada soyutlanır
        public void Send(string to, string subject, string body)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(to, nameof(to));
            ArgumentException.ThrowIfNullOrWhiteSpace(subject, nameof(subject));
            ArgumentException.ThrowIfNullOrWhiteSpace(body, nameof(body));

            Console.WriteLine($" [ EMAIL -> {to}]");
            Console.WriteLine($"  Konu : {subject}");
            Console.WriteLine($"  İçerik: {body}");
        }
    }
}
