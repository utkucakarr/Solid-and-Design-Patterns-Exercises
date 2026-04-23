using State_Implementation.Interfaces;

namespace State_Implementation.Models
{
    // Context sınıfı: state'i tutar ve delege eder
    public class OrderContext : IOrderContext
    {
        public string OrderId { get; }
        public decimal Amount { get; }
        public IOrderState CurrentState { get; private set; }

        public OrderContext(string orderId, decimal amount, IOrderState initialState)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(orderId, nameof(orderId));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));
            ArgumentNullException.ThrowIfNull(initialState, nameof(initialState));

            OrderId = orderId;
            Amount = amount;
            CurrentState = initialState;
        }

        // State geçişi merkezi bir noktadan yönetilir
        public void TransitionTo(IOrderState newState)
        {
            ArgumentNullException.ThrowIfNull(newState, nameof(newState));
            CurrentState = newState;
        }

        // Tüm işlemler aktif state'e delege edilir — if/switch yok
        public OrderResult Confirm() => CurrentState.Confirm(this);
        public OrderResult Ship() => CurrentState.Ship(this);
        public OrderResult Deliver() => CurrentState.Deliver(this);
        public OrderResult Cancel() => CurrentState.Cancel(this);
        public string GetStatusDescription() => CurrentState.GetStatusDescription();
    }
}
