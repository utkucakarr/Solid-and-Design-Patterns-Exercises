using DIP_Implementation.Interfaces;
using DIP_Implementation.Models;

namespace DIP_Implementation.Services
{
    public class SmsNotificationService : INotificationService
    {
        public string Channel => "SMS";

        public NotificationResult Send(string message)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            Console.WriteLine($"[SMS] {message}");
            return NotificationResult.Success(Channel, message);
        }
    }
}