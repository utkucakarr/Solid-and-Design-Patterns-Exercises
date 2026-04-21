using Observer_Implementation.Interfaces;

namespace Observer_Implementation.Subject
{
    public class PushNotifier : IPushNotifier
    {
        // Push bildirimi tek sorumluluğa sahip — gerçek FCM/APNs burada soyutlanır
        public void Send(string deviceToken, string title, string body)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(deviceToken, nameof(deviceToken));
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(body, nameof(body));

            Console.WriteLine($" [ PUSH -> {deviceToken}]");
            Console.WriteLine($"   Başlık: {title}");
            Console.WriteLine($"   İçerik: {body}");
        }
    }
}
