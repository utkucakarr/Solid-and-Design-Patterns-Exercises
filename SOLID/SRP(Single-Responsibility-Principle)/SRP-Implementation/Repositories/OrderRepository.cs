using SRP_Implementation.Interfeces;
using SRP_Implementation.Models;

namespace SRP_Implementation.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new();

        public Order? GetById(int id)
            => _orders.FirstOrDefault(o => o.Id == id);

        public void Save(Order order)
        {
            _orders.Add(order);
            Console.WriteLine($"[DB] Order {order.Id} kaydedildi.");
        }
    }
}
