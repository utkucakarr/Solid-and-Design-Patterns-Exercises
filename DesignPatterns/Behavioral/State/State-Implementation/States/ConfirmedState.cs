using State_Implementation.Interfaces;
using State_Implementation.Models;

namespace State_Implementation.States
{
    // Confirmed state: sadece kendi geçişlerini yönetir
    public class ConfirmedState : IOrderState
    {
        public OrderResult Confirm(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "Sipariş zaten onaylanmış.",
                GetStatusDescription());
        }

        public OrderResult Ship(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            var from = GetStatusDescription();
            // Geçerli geçiş: Confirmed → Shipped
            context.TransitionTo(new ShippedState());
            return OrderResult.Success(context.OrderId,
                "Sipariş kargoya verildi. Takip kodu oluşturuldu.",
                from, context.GetStatusDescription());
        }

        public OrderResult Deliver(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "Sipariş kargoya verilmeden teslim edilemez.",
                GetStatusDescription());
        }

        public OrderResult Cancel(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            var from = GetStatusDescription();
            // Geçerli geçiş: Confirmed → Cancelled
            context.TransitionTo(new CancelledState());
            return OrderResult.Success(context.OrderId,
                "Sipariş onay aşamasında iptal edildi. İade başlatıldı.",
                from, context.GetStatusDescription());
        }

        public string GetStatusDescription() => "Sipariş Onaylandı";
    }
}
