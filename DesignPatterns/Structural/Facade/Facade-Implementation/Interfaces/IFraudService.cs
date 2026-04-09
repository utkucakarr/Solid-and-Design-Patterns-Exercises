namespace Facade_Implementation.Interfaces
{
    public interface IFraudService
    {
        bool IsSuspicious(string accountId, decimal amount);
    }
}
