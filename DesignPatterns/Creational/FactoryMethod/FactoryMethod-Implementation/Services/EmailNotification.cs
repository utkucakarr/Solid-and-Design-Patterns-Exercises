using FactoryMethod_Implementation.Interfaces;
using FactoryMethod_Implementation.Models;

namespace FactoryMethod_Implementation.Services
{
    public class EmailNotification : INotification
    {
        public string Channel => "Email";

        public NotificationResult Send(string recipient, string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(recipient, nameof(recipient));
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            Console.WriteLine($"[EMAIL] {recipient} -> {message}");
            return NotificationResult.Success(Channel, recipient, message);
        }
    }
}
