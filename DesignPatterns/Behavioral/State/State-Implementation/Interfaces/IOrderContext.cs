using State_Implementation.Models;

namespace State_Implementation.Interfaces
{
    public interface IOrderContext
    {
        string OrderId { get; }
        decimal Amount { get; }
        IOrderState CurrentState { get; }

        OrderResult Confirm();
        OrderResult Ship();
        OrderResult Deliver();
        OrderResult Cancel();
        string GetStatusDescription();

        // State geçişi context üzerinden yönetilir
        void TransitionTo(IOrderState newState);
    }
}
