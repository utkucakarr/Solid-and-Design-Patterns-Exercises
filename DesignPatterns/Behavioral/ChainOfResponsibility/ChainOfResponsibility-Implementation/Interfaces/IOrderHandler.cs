using ChainOfResponsibility_Implementation.Models;

namespace ChainOfResponsibility_Implementation.Interfaces
{
    public interface IOrderHandler
    {
        IOrderHandler SetNext(IOrderHandler next);
        OrderResult Handle(OrderRequest request);
    }
}
