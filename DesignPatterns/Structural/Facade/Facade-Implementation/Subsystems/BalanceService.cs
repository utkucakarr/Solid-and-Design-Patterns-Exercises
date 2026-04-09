using Facade_Implementation.Interfaces;

namespace Facade_Implementation.Subsystems
{
    public class BalanceService : IBalanceService
    {
        public void AddBalance(string accountId, decimal amount)
            => Console.WriteLine($"[Bakiye] {accountId} hesabına {amount} TL eklendi.");

        public void DeductBalance(string accountId, decimal amount)
            => Console.WriteLine($"[Bakiye] {accountId} hesabından {amount} TL düşüldü.");

        public decimal GetBalance(string accountId)
        {
            Console.WriteLine($"[Bakiye] {accountId} bakiyesi sorgulanıyor.");
            return 10_000m;
        }
    }
}
