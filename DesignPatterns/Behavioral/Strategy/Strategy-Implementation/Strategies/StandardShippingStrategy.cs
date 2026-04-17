using Strategy_Implementation.Interfaces;
using Strategy_Implementation.Models;

namespace Strategy_Implementation.Strategies
{
    public sealed class StandardShippingStrategy : IShippingStrategy
    {
        private const decimal BaseFee = 15.00m;
        private const decimal WeightRate = 2.50m;

        public string StrategyName => "Standart Kargo";

        // Sadece standart kargo algoritması burada
        public ShippingResult Calculate(ShippingOrder order)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));
            ArgumentException.ThrowIfNullOrWhiteSpace(order.OrderId, nameof(order.OrderId));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(order.WeightKg, nameof(order.WeightKg));

            decimal cost = BaseFee + ((decimal)order.WeightKg * WeightRate);

            return ShippingResult.Success(
                cost: cost,
                carrierName: "Mng Kargo",
                estimatedDays: 3,
                strategyUsed: StrategyName
                );
        }
    }
}
