using Observer_Implementation.Interfaces;
using Observer_Implementation.Models;

namespace Observer_Implementation.Subject
{
    public class OrderService : IOrderSubject, IOrderService
    {
        private readonly List<IOrderObserver> _observers = new();
        private readonly List<Order> _orders = new();

        // Subscribe — yeni observer dinamik olarak eklenir
        public void Subscribe(IOrderObserver observer)
        {
            ArgumentNullException.ThrowIfNull(observer, nameof(observer));

            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        // Unsubscribe — observer dinamik olarak çıkarılır
        public void Unsubscribe(IOrderObserver observer)
        {
            ArgumentNullException.ThrowIfNull(observer, nameof(observer));
            _observers.Remove(observer);
        }

        // Tüm observer'lar bilgilendirilir — OrderService kimin ne yaptığını bilmez
        public void NotifyObservers(Order order, OrderStatus previousStatus)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));

            foreach (var observer in _observers)
                observer.OnOrderStatusChanged(order, previousStatus);
        }

        // Sipariş oluşturma — Placed durumunda observer'lar tetiklenir
        public OrderResult PlaceOrder(Order order)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));

            _orders.Add(order);

            NotifyObservers(order, OrderStatus.Placed);

            return OrderResult.Success(order.OrderId, order.Status, _observers.Count);
        }

        // Durum değişimi — önceki durum saklanır, observer'lar bilgilendirilir
        public OrderResult ChangeOrderStatus(string orderId, OrderStatus newStatus)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(orderId, nameof(orderId));

            var order = _orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order is null)
                return OrderResult.Fail($"{orderId} numaralı sipariş bulunamadı.");

            if (order.Status == newStatus)
                return OrderResult.Fail(
                    $"Sipariş zaten '{newStatus}' durumunda.");

            var previousStatus = order.Status;

            // Durum güncellenir
            order.UpdateStatus(newStatus);

            // Observer'lar tetiklenir — OrderService iş mantığı bilmez
            NotifyObservers(order, previousStatus);

            return OrderResult.Success(order.OrderId, order.Status, _observers.Count);
        }
    }
}
