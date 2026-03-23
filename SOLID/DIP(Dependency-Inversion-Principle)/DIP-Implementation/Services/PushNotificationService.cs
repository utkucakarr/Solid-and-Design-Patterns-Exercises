using DIP_Implementation.Interfaces;
using DIP_Implementation.Models;

namespace DIP_Implementation.Services
{
    public class PushNotificationService : INotificationService
    {
        public string Channel => "PUSH";

        public NotificationResult Send(string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            Console.WriteLine($"[PUSH] {message}");
            return NotificationResult.Success(Channel, message);
        }
    }
}