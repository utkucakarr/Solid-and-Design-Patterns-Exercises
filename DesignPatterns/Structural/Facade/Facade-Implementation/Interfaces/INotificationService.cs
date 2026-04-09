namespace Facade_Implementation.Interfaces
{
    public interface INotificationService
    {
        void NotifySender(string accountId, decimal amount, string referenceId);

        void NotifyReceiver(string accountId, decimal amount, string referenceId);
    }
}
