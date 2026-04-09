namespace Facade_Violation.Subsystems
{
    public class FraudServiceBad
    {
        public bool IsSuspicious(string accountId, decimal amount)
        {
            Console.WriteLine($"[Fraud] {accountId} için {amount} TL fraud kontrolü yapılıyor.");
            return false; // Gerçekte ML modeli çalışır
        }
    }
}
