using Observer_Implementation.Interfaces;
using Observer_Implementation.Models;

namespace Observer_Implementation.Observers
{
    public class PushNotificationObserver : IOrderObserver
    {
        private readonly IPushNotifier _pushNotifier;

        public PushNotificationObserver(IPushNotifier pushNotifier)
        {
            ArgumentNullException.ThrowIfNull(pushNotifier, nameof(pushNotifier));
            _pushNotifier = pushNotifier;
        }

        public void OnOrderStatusChanged(Order order, OrderStatus previousStatus)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));

            // Push bildirimi yalnızca Shipped ve Delivered durumlarında tetiklenir
            if (order.Status is not (OrderStatus.Shipped or OrderStatus.Delivered))
                return;

            var (title, body) = order.Status switch
            {
                OrderStatus.Shipped =>
                    (" Siparişiniz Yola Çıktı!",
                     $"{order.OrderId} numaralı siparişiniz kargoya verildi."),
                OrderStatus.Delivered =>
                    ("Siparişiniz Teslim Edildi!",
                     $"{order.OrderId} numaralı siparişiniz kapınıza ulaştı."),
                _ => (string.Empty, string.Empty)
            };

            // Push bildirimi yalnızca bu observer'ın sorumluluğunda
            _pushNotifier.Send(order.CustomerDeviceToken, title, body);
        }
    }
}
