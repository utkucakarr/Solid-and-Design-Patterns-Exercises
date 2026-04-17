using Strategy_Implementation.Models;

namespace Strategy_Implementation.Interfaces
{
    public interface IShippingStrategy
    {
        // Her strategy aynı sözleşmeye uymak zorunda.
        string StrategyName { get; }
        ShippingResult Calculate(ShippingOrder order);
    }
}
