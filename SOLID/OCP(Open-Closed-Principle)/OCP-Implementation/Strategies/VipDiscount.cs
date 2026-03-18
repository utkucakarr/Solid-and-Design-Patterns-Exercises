using OCP_Implementation.Interfaces;

namespace OCP_Implementation.Strategies
{
    public class VipDiscount : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal amount) => amount * 0.20m;
    }
}
