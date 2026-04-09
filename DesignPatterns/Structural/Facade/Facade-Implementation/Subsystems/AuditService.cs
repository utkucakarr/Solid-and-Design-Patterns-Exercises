using Facade_Implementation.Interfaces;

namespace Facade_Implementation.Subsystems
{
    public class AuditService : IAuditService
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
