using Strategy_Implementation.Interfaces;
using Strategy_Implementation.Models;

namespace Strategy_Implementation.Strategies
{
    public sealed class ExpressingShippingStrategy : IShippingStrategy
    {
        private const decimal BaseFee = 35.00m;
        private const decimal WeightRate = 5.00m;

        public string StrategyName => "Hızlı Kargo";

        public ShippingResult Calculate(ShippingOrder order)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));
            ArgumentException.ThrowIfNullOrWhiteSpace(order.OrderId, nameof(order.OrderId));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(order.WeightKg, nameof(order.WeightKg));

            decimal cost = BaseFee + ((decimal)order.WeightKg * WeightRate);

            return ShippingResult.Success(
                cost: cost,
                carrierName: "Yurtiçi Kargo",
                estimatedDays: 1,
                strategyUsed: StrategyName
                );
        }
    }
}