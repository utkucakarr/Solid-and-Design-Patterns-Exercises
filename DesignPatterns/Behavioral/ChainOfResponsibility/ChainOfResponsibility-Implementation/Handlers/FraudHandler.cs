using ChainOfResponsibility_Implementation.Models;

namespace ChainOfResponsibility_Implementation.Handlers
{
    // Sadece fraud kontrolünden sorumlu
    public sealed class FraudHandler : BaseOrderHandler
    {
        private const decimal FraudThreshold = 75m;

        private readonly Dictionary<string, decimal> _fraudScores = new()
        {
            ["CUST-001"] = 15m,
            ["CUST-FRAUD"] = 90m,
            ["CUST-002"] = 30m
        };

        public override OrderResult Handle(OrderRequest request)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var score = _fraudScores.GetValueOrDefault(request.CustomerId, 50m);

            if (score > FraudThreshold)
            {
                // Zincir burada kırılır
                return OrderResult.Fail(
                    $"Şüpheli işlem tespit edildi. Fraud skoru: {score}",
                    nameof(FraudHandler));
            }

            Console.WriteLine($"[FraudHandler] Fraud skoru normal: {score}");
            return PassToNext(request);
        }
    }
}
