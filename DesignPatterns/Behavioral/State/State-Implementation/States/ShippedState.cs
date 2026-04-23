using State_Implementation.Interfaces;
using State_Implementation.Models;

namespace State_Implementation.States
{
    // Shipped state: kargodaki siparişin kurallarını kapsar
    public class ShippedState : IOrderState
    {
        public OrderResult Confirm(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "Sipariş kargoda, tekrar onaylanamaz.",
                GetStatusDescription());
        }

        public OrderResult Ship(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "Sipariş zaten kargoda.",
                GetStatusDescription());
        }

        public OrderResult Deliver(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            var from = GetStatusDescription();
            // Geçerli geçiş: Shipped → Delivered
            context.TransitionTo(new DeliveredState());
            return OrderResult.Success(context.OrderId,
                "Sipariş teslim edildi. Müşteri bilgilendirildi.",
                from, context.GetStatusDescription());
        }

        public OrderResult Cancel(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            // İş kuralı kapsüllendi: kargodaki sipariş iptal edilemez
            return OrderResult.Fail(context.OrderId,
                "Sipariş kargoda olduğu için iptal edilemez. Teslimat sonrası iade talebi açılabilir.",
                GetStatusDescription());
        }

        public string GetStatusDescription() => "Kargoya Verildi";
    }
}
