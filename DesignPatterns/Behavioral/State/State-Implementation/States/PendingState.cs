using State_Implementation.Interfaces;
using State_Implementation.Models;

namespace State_Implementation.States
{
    // Pending state: sadece kendi sorumluluğunu bilir
    public class PendingState : IOrderState
    {
        public OrderResult Confirm(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            var from = GetStatusDescription();
            // Geçerli geçiş: Pending → Confirmed
            context.TransitionTo(new ConfirmedState());
            return OrderResult.Success(context.OrderId,
                $"Sipariş onaylandı. Ödeme alındı: {context.Amount:C}",
                from, context.GetStatusDescription());
        }

        public OrderResult Ship(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            // Geçersiz geçiş: kural bu state içinde kapsüllendi
            return OrderResult.Fail(context.OrderId,
                "Sipariş henüz onaylanmadı, kargoya verilemez.",
                GetStatusDescription());
        }

        public OrderResult Deliver(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "Sipariş henüz onaylanmadı, teslim edilemez.",
                GetStatusDescription());
        }

        public OrderResult Cancel(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            var from = GetStatusDescription();
            // Geçerli geçiş: Pending → Cancelled
            context.TransitionTo(new CancelledState());
            return OrderResult.Success(context.OrderId,
                "Sipariş ödeme onayı beklenmeden iptal edildi.",
                from, context.GetStatusDescription());
        }

        public string GetStatusDescription() => "Ödeme Bekleniyor";
    }
}
