using SRP_Implementation.Interfeces;
using SRP_Implementation.Models;

namespace SRP_Implementation.Services
{
    public class OrderNotificationService : INotificationService
    {
        public void SendConfirmation(Order order)
            => Console.WriteLine($"[EMAIL] {order.CustomerEmail} adresine onay gönderildi.");
    }
}
