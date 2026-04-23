using State_Implementation.Interfaces;
using State_Implementation.Models;

namespace State_Implementation.States
{
    // Delivered state: terminal state, yalnızca hata döner
    public class DeliveredState : IOrderState
    {
        public OrderResult Confirm(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "Sipariş teslim edildi, işlem yapılamaz.",
                GetStatusDescription());
        }

        public OrderResult Ship(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "Sipariş teslim edildi, işlem yapılamaz.",
                GetStatusDescription());
        }

        public OrderResult Deliver(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "Sipariş zaten teslim edildi.",
                GetStatusDescription());
        }

        public OrderResult Cancel(OrderContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            return OrderResult.Fail(context.OrderId,
                "Teslim edilmiş sipariş iptal edilemez. İade talebi oluşturabilirsiniz.",
                GetStatusDescription());
        }

        public string GetStatusDescription() => "Teslim Edildi";
    }
}
