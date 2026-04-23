using ChainOfResponsibility_Implementation.Models;

namespace ChainOfResponsibility_Implementation.Handlers
{
    // Sadece ödeme kontrolünden sorumlu
    public sealed class PaymentHandler : BaseOrderHandler
    {
        private const decimal UnitPrice = 299.99m;

        public override OrderResult Handle(OrderRequest request)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var totalPrice = UnitPrice * request.Quantity;

            if (request.AccountBalance < totalPrice)
            {
                // Zincir burada kırılır
                return OrderResult.Fail(
                    $"Bakiye yetersiz. Gereken: {totalPrice:C}, Mevcut: {request.AccountBalance:C}",
                    nameof(PaymentHandler));
            }

            Console.WriteLine($"PaymentHandler] Ödeme onaylandı: {totalPrice:C}");
            return PassToNext(request);
        }
    }
}
