using Facade_Implementation.Interfaces;

namespace Facade_Implementation.Subsystems
{
    public class NotificationService : INotificationService
    {
        public void NotifyReceiver(string accountId, decimal amount, string referenceId)
            => Console.WriteLine($"[Bildirim] Gönderen {accountId}: {amount} TL gönderildi. Ref: {referenceId}");

        public void NotifySender(string accountId, decimal amount, string referenceId)
            => Console.WriteLine($"[Bildirim] Alıcı {accountId}: {amount} TL alındı. Ref: {referenceId}");
    }
}