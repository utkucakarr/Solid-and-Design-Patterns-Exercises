using Strategy_Implementation.Interfaces;
using Strategy_Implementation.Models;

namespace Strategy_Implementation.Strategies
{
    public sealed class MemberShippingStrategy : IShippingStrategy
    {
        private const string RequiredMemberShip = "Premium";
        private const decimal BaseFee = 15.00m;
        private const decimal WeightRate = 2.50m;
        private const decimal DiscountRate = 0.40m;


        public string StrategyName => "Premium Üye Kargosu";

        public ShippingResult Calculate(ShippingOrder order)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));
            ArgumentException.ThrowIfNullOrWhiteSpace(order.OrderId, nameof(order.OrderId));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(order.WeightKg, nameof(order.WeightKg));

            if(!order.MembershipType.Equals(RequiredMemberShip, StringComparison.OrdinalIgnoreCase))
            {
                return ShippingResult.Fail(
                    $"Bu kargo tipi yalnızca '{RequiredMemberShip}' üyelere özeldir. " +
                    $"Mevcut üyelik: '{order.MembershipType}'");
            }

            decimal baseCost = BaseFee + ((decimal)order.WeightKg * WeightRate);
            decimal discountedCost = baseCost * (1 - DiscountRate);

            return ShippingResult.Success(
                cost: baseCost,
                carrierName: "PTT Kargo",
                estimatedDays: 4,
                strategyUsed: StrategyName,
                message: $"%{DiscountRate * 100} Premium üye indirimi uygulandı.");
        }
    }
}
