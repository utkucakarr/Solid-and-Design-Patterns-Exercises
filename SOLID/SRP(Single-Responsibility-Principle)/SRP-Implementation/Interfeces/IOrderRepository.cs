using SRP_Implementation.Models;

namespace SRP_Implementation.Interfeces
{
    public interface IOrderRepository
    {
        void Save(Order order);
        Order? GetById(int id);
    }
}
