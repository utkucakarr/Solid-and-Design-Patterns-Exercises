using Observer_Implementation.Interfaces;

namespace Observer_Implementation.Subject
{
    public class SmsNotifier : ISmsNotifier
    {
        // SMS gönderimi tek sorumluluğa sahip — gerçek SMS gateway burada soyutlanır
        public void Send(string phoneNumber, string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            Console.WriteLine($" [ SMS -> {phoneNumber}]");
            Console.WriteLine($"   Mesaj: {message}");
        }
    }
}
