using DIP_Implementation.Interfaces;
using DIP_Implementation.Models;

namespace DIP_Implementation.Services
{
    public class EmailNotificationService : INotificationService
    {
        public string Channel => "EMAIL";

        public NotificationResult Send(string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            Console.WriteLine($"[EMAIL] {message}");
            return NotificationResult.Success(Channel, message);
        }
    }
}
