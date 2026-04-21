using Observer_Implementation.Models;

namespace Observer_Implementation.Interfaces
{
    public interface IOrderObserver
    {
        void OnOrderStatusChanged(Order order, OrderStatus previousStatus);
    }
}
