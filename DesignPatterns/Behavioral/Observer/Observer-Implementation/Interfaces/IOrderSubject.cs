using Observer_Implementation.Models;

namespace Observer_Implementation.Interfaces
{
    public interface IOrderSubject
    {
        void Subscribe(IOrderObserver observer);
        void Unsubscribe(IOrderObserver observer);
        void NotifyObservers(Order order, OrderStatus previousStatus);
    }
}
