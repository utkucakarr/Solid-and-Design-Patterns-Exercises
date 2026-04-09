using Facade_Implementation.Interfaces;

namespace Facade_Implementation.Subsystems
{
    public class FraudService : IFraudService
    {
        public bool IsSuspicious(string accountId, decimal amount)
        {
            Console.WriteLine($"[Fraud] {accountId} için {amount} TL fraud kontrolü yapılıyor.");
            return false;
        }
    }
}
