using Strategy_Implementation.Models;

namespace Strategy_Implementation.Interfaces
{
    public interface IShippingContext
    {
        // Context stratejiyi dışarıdan alır - algoritma bilgisi yok
        void SetStrategy(IShippingStrategy strategy);
        ShippingResult ExecuteShipping(ShippingOrder order);
    }
}