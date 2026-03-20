using SRP_Implementation.Interfeces;
using SRP_Implementation.Models;

namespace SRP_Implementation.Logging
{
    public class OrderLogger : IOrderLogger
    {
        public void LogOrderCreated(Order order)
            => Console.WriteLine($"[LOG] Order {order.Id} işlendi. Tarih: {DateTime.Now}");
    }
}
