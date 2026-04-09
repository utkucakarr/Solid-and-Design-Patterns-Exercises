namespace Facade_Violation.Subsystems
{
    public class AuditServiceBad
    {
        public void LogTransfer(
            string fromAccount, 
            string toAccount, 
            decimal amount, 
            string referenceId)
            => Console.WriteLine($"[Audit] Transfer loglandı. Ref: {referenceId} | " + 
                $"{fromAccount} -> {toAccount} : {amount} TL");
    }
}
