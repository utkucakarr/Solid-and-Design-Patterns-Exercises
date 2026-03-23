using DIP_Implementation.Interfaces;
using DIP_Implementation.Models;

namespace DIP_Implementation.Services
{
    public class NotificationManager
    {
        private readonly IEnumerable<INotificationService> _services;

        // ✅ Constructor injection — bağımlılıklar dışarıdan geliyor
        public NotificationManager(IEnumerable<INotificationService> services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        // ✅ Yeni kanal eklemek için bu sınıfa dokunmak gerekmez!
        public IEnumerable<NotificationResult> SendAll(string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            var results = new List<NotificationResult>();

            foreach (var service in _services)
            {
                var result = service.Send(message);
                results.Add(result);
            }

            return results;
        }

        public NotificationResult? SendToChannel(string channel, string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(channel, nameof(channel));
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            var service = _services
                .FirstOrDefault(s => s.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase));

            if (service is null)
                return NotificationResult.Fail(channel, "Kanal bulunamadı.");

            return service.Send(message);
        }
    }
}
