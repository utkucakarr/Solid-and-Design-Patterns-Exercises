using Observer_Implementation.Interfaces;
using Observer_Implementation.Models;

namespace Observer_Implementation.Observers
{
    public class EmailNotificationObserver : IOrderObserver
    {
        private readonly IEmailNotifier _emailNotifier;

        public EmailNotificationObserver(IEmailNotifier emailNotifier)
        {
            ArgumentNullException.ThrowIfNull(emailNotifier, nameof(emailNotifier));
            _emailNotifier = emailNotifier;
        }

        public void OnOrderStatusChanged(Order order, OrderStatus previousStatus)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));

            var subject = order.Status switch
            {
                OrderStatus.Confirmed => $"Siparişiniz {order.OrderId} Onaylandı",
                OrderStatus.Shipped => $"Siparişiniz {order.OrderId} Kargoya Verildi",
                OrderStatus.Delivered => $"Siparişiniz {order.OrderId} Teslim Edildi",
                OrderStatus.Cancelled => $"Siparişiniz {order.OrderId} İptal Edildi",
                _ => $"Siparişiniz #{order.OrderId} Güncellendi"
            };

            var body = order.Status switch
            {
                OrderStatus.Confirmed =>
                    $"Merhaba, {order.ProductName} ürününü içeren siparişiniz onaylandı. " +
                    $"Toplam tutar: {order.TotalPrice}",
                OrderStatus.Shipped =>
                    $"Merhaba, siparişiniz kargoya verildi. " +
                    $"Takip kodu yakında tarafınıza iletilecek.",
                OrderStatus.Delivered =>
                    $"Merhaba, siparişiniz teslim edildi. " +
                    $"Alışverişiniz için teşekkür ederiz!",
                OrderStatus.Cancelled =>
                    $"Merhaba, siparişiniz iptal edildi. " +
                    $"İade işleminiz 3-5 iş günü içinde tamamlanacak.",
                _ =>
                    $"Siparişinizin durumu '{previousStatus}' -> '{order.Status}' olarak güncellendi."
            };

            // E-posta gönderimi yalnızca bu observer'ın sorumluluğunda
            _emailNotifier.Send(order.CustomerEmail, subject, body);
        }
    }
}
