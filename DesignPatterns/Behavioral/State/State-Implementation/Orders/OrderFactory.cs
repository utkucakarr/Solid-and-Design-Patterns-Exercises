using State_Implementation.Interfaces;
using State_Implementation.Models;
using State_Implementation.States;

namespace State_Implementation.Orders
{
    // Yeni sipariş oluşturmayı merkezi bir noktada toplar
    public class OrderFactory : IOrderFactory
    {
        public OrderContext Create(string orderId, decimal amount)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(orderId, nameof(orderId));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));

            // Her yeni sipariş PendingState ile başlar
            return new OrderContext(orderId, amount, new PendingState());
        }
    }
}
