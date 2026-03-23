using DIP_Implementation.Models;

namespace DIP_Implementation.Interfaces
{
    public interface INotificationService
    {
        string Channel { get; }
        NotificationResult Send(string message);
    }
}
