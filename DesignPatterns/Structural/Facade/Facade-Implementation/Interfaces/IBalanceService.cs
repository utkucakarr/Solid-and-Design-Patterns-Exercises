namespace Facade_Implementation.Interfaces
{
    public interface IBalanceService
    {
        decimal GetBalance(string accountId);
        void DeductBalance(string accountId, decimal amount);
        void AddBalance(string accountId, decimal amount);
    }
}