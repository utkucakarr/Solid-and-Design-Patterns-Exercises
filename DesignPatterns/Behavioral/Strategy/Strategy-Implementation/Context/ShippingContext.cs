using Strategy_Implementation.Interfaces;
using Strategy_Implementation.Models;

namespace Strategy_Implementation.Context
{
    public sealed class ShippingContext : IShippingContext
    {
        private IShippingStrategy? _strategy;

        // Strateji runtime'da enjekte edilir — context algoritma bilmez
        public ShippingResult ExecuteShipping(ShippingOrder order)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));

            if (_strategy is null)
                return ShippingResult.Fail("Kargo stratejisi belirlenmemiş. Lütfen önce SetStrategy() çağırın.");

            // Hangi strateji olursa olsun aynı çağrı — polymorphism
            return _strategy.Calculate(order);
        }

        public void SetStrategy(IShippingStrategy strategy)
        {
            ArgumentNullException.ThrowIfNull(strategy, nameof(strategy));
            _strategy = strategy;
        }
    }
}
