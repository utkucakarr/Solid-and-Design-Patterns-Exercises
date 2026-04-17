using Strategy_Implementation.Interfaces;
using Strategy_Implementation.Models;

namespace Strategy_Implementation.Strategies
{
    public sealed class FreeShippingStrategy : IShippingStrategy
    {
        private const decimal MinimumOrderTotal = 500.00m;
        public string StrategyName => "Ücretsiz Kargo";

        public ShippingResult Calculate(ShippingOrder order)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));
            ArgumentException.ThrowIfNullOrWhiteSpace(order.OrderId, nameof(order.OrderId));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(order.WeightKg, nameof(order.WeightKg));

            if(order.OrderTotal < MinimumOrderTotal)
            {
                return ShippingResult.Fail(
                    $"Ücretsiz kargo için minimum {MinimumOrderTotal:C} sipariş tutarı gereklidir. " +
                    $"Mevcut tutar: {order.OrderTotal:C}");
            }

            return ShippingResult.Success(
                cost: 0m,
                carrierName: "Aras Kargo",
                estimatedDays: 5,
                strategyUsed: StrategyName,
                message: "Ücretsiz kargo uygulandı.");
        }
    }
}
