namespace Facade_Violation.Subsystems
{
    public class NotificationServiceBad
    {
        public void NotifySender(string accountId, decimal amount, string referenceId)
       => Console.WriteLine($"[Bildirim] Gönderen {accountId}: {amount} TL gönderildi. Ref: {referenceId}");

        public void NotifyReceiver(string accountId, decimal amount, string referenceId)
            => Console.WriteLine($"[Bildirim] Alıcı {accountId}: {amount} TL alındı. Ref: {referenceId}");
    }
}
