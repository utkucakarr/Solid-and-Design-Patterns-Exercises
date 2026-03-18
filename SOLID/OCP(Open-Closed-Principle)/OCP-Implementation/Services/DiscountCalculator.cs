using OCP_Implementation.Interfaces;

namespace OCP_Implementation.Services
{
    public class DiscountCalculator
    {
        // Bu sınıf artık hiçbir zaman değişmeyecek. 
        // Yeni kurallar sadece yeni 'IDiscountStrategy' sınıfları olarak eklenecek.
        public decimal Calculate(decimal amount, IDiscountStrategy strategy)
        {
            if (amount <= 0) 
                return 0;

            return strategy.ApplyDiscount(amount);
        }
    }
}
