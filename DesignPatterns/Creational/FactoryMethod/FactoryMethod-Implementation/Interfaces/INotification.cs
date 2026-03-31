using FactoryMethod_Implementation.Models;

namespace FactoryMethod_Implementation.Interfaces
{
    // Tüm bildirim tiplerinin ortak sözleşmesi
    public interface INotification
    {
        string Channel { get; }
        NotificationResult Send(string recipient, string message);
    }
}
