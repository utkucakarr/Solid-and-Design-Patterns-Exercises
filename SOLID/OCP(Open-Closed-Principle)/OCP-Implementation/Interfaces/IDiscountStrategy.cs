namespace OCP_Implementation.Interfaces
{
    public interface IDiscountStrategy
    {
        decimal ApplyDiscount(decimal amount);
    }
}
