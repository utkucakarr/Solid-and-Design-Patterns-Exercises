using FactoryMethod_Implementation.Interfaces;
using FactoryMethod_Implementation.Models;

namespace FactoryMethod_Implementation.Services
{
    public class SmsNotification : INotification
    {
        public string Channel => "Sms";

        public NotificationResult Send(string recipient, string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(recipient, nameof(recipient));
            ArgumentException.ThrowIfNullOrWhiteSpace(recipient, nameof(message));

            Console.WriteLine($"[SMS] {recipient} -> {message}");
            return NotificationResult.Success(Channel, recipient, message);
        }
    }
}
