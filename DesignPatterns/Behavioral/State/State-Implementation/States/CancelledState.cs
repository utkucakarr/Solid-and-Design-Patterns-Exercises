using State_Implementation.Interfaces;
using State_Implementation.Models;

namespace State_Implementation.States
{
    // Cancelled state: terminal state, tüm işlemler reddedilir
    public class CancelledState : IOrderState
    {
        public OrderResult Confirm(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "İptal edilmiş sipariş onaylanamaz.",
                GetStatusDescription());
        }

        public OrderResult Ship(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "İptal edilmiş sipariş kargoya verilemez.",
                GetStatusDescription());
        }

        public OrderResult Deliver(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "İptal edilmiş sipariş teslim edilemez.",
                GetStatusDescription());
        }

        public OrderResult Cancel(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "Sipariş zaten iptal edilmiş.",
                GetStatusDescription());
        }

        public string GetStatusDescription() => "İptal Edildi";
    }
}
