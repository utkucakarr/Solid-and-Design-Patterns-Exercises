using OCP_Implementation.Interfaces;

namespace OCP_Implementation.Strategies
{
    public class PremiumDiscount : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal amount) => amount * 0.10m;
    }
}
