using Observer_Implementation.Interfaces;
using Observer_Implementation.Models;

namespace Observer_Implementation.Observers
{
    public class SmsNotificationObserver : IOrderObserver
    {
        private readonly ISmsNotifier _smsNotifier;

        public SmsNotificationObserver(ISmsNotifier smsNotifier)
        {
            ArgumentNullException.ThrowIfNull(smsNotifier, nameof(smsNotifier));
            _smsNotifier = smsNotifier;
        }

        public void OnOrderStatusChanged(Order order, OrderStatus previousStatus)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));

            // Yalnızca kritik durumlarda SMS gönderilir — gereksiz bildirim önlenir
            if (order.Status is not (OrderStatus.Confirmed
                or OrderStatus.Shipped
                or OrderStatus.Delivered
                or OrderStatus.Cancelled))
                return;

            var message = order.Status switch
            {
                OrderStatus.Confirmed =>
                    $"Siparişiniz #{order.OrderId} onaylandı. Tutar: {order.TotalPrice:C}",
                OrderStatus.Shipped =>
                    $"Siparişiniz #{order.OrderId} kargoya verildi.",
                OrderStatus.Delivered =>
                    $"Siparişiniz #{order.OrderId} teslim edildi. Teşekkürler!",
                OrderStatus.Cancelled =>
                    $"Siparişiniz #{order.OrderId} iptal edildi.",
                _ => string.Empty
            };


            // SMS gönderimi yalnızca bu observer'ın sorumluluğunda
            _smsNotifier.Send(order.CustomerPhone, message);
        }
    }
}
