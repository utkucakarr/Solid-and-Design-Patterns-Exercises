using State_Implementation.Models;

namespace State_Implementation.Interfaces
{
    public interface IOrderState
    {
        // Her state kendi davranışını implemente eder
        OrderResult Confirm(OrderContext context);
        OrderResult Ship(OrderContext context);
        OrderResult Deliver(OrderContext context);
        OrderResult Cancel(OrderContext context);
        string GetStatusDescription();
    }
}
