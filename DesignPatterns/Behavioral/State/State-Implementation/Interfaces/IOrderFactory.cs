using State_Implementation.Models;

namespace State_Implementation.Interfaces
{
    public interface IOrderFactory
    {
        OrderContext Create(string orderId, decimal amount);
    }
}
