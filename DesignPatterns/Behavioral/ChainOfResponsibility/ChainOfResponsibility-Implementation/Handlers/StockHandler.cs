using ChainOfResponsibility_Implementation.Models;

namespace ChainOfResponsibility_Implementation.Handlers
{
    public sealed class StockHandler : BaseOrderHandler
    {
        private readonly Dictionary<string, int> _stockMap = new()
        {
            ["PROD-001"] = 50,
            ["PROD-002"] = 0,
            ["PROD-003"] = 10,
            ["DIGITAL-ONLY"] = int.MaxValue
        };

        public override OrderResult Handle(OrderRequest request)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var stock = _stockMap.GetValueOrDefault(request.ProductId, 0);

            if (stock < request.Quantity)
            {
                // Zincir burada kırılır, sonraki handler çağrılmaz
                return OrderResult.Fail(
                    $"Stok yetersiz. Mevcut: {stock}, İstenen: {request.Quantity}",
                    nameof(StockHandler));
            }

            Console.WriteLine($"[StockHandler] Stok yeterli: {stock} adet mevcut.");
            return PassToNext(request);
        }
    }
}