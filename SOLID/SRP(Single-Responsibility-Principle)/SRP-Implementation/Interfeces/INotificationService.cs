using SRP_Implementation.Models;

namespace SRP_Implementation.Interfeces
{
    public interface INotificationService
    {
        void SendConfirmation(Order order);
    }
}
