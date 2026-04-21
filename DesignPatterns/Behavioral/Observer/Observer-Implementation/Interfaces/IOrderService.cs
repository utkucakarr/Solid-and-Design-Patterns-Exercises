using Observer_Implementation.Models;

namespace Observer_Implementation.Interfaces
{
    public interface IOrderService
    {
        OrderResult PlaceOrder(Order order);
        OrderResult ChangeOrderStatus(string orderId, OrderStatus newStatus);
    }
}
