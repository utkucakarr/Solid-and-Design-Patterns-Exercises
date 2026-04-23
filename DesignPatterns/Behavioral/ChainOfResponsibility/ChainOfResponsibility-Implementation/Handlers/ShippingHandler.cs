using ChainOfResponsibility_Implementation.Models;

namespace ChainOfResponsibility_Implementation.Handlers
{
    // Sadece kargo kontrolünden sorumlu
    public sealed class ShippingHandler : BaseOrderHandler
    {
        private readonly HashSet<string> _noShippingProducts = ["DIGITAL-ONLY"];

        public override OrderResult Handle(OrderRequest request)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (_noShippingProducts.Contains(request.ProductId))
            {
                // Zincir burada kırılır
                return OrderResult.Fail(
                    "Bu ürün için fiziksel kargo hizmeti mevcut değil.",
                    nameof(ShippingHandler));
            }

            Console.WriteLine("[ShippingHandler] Kargo oluşturulabilir.");
            return PassToNext(request); // Son handler — tracking code üretilir
        }
    }
}
