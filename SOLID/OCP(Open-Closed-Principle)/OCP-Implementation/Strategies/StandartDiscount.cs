using OCP_Implementation.Interfaces;

namespace OCP_Implementation.Strategies
{
    public class StandartDiscount : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal amount) => amount * 0.05m;
    }
}
