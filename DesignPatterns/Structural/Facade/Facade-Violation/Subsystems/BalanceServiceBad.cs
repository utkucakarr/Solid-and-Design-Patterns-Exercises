namespace Facade_Violation.Subsystems
{
    public class BalanceServiceBad
    {
        public decimal GetBalance(string accountId)
        {
            Console.WriteLine($"[Bakiye] {accountId} bakiyesi sorgulanıyor.");
            return 10_000m; // Gerçekte DB'den gelir
        }

        public void DeductBalance(string accountId, decimal amount)
            => Console.WriteLine($"[Bakiye] {accountId} hesabından {amount} TL düşüldü.");

        public void AddBalance(string accountId, decimal amount)
            => Console.WriteLine($"[Bakiye] {accountId} hesabına {amount} TL eklendi.");
    }
}
