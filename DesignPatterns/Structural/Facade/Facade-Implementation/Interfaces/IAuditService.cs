namespace Facade_Implementation.Interfaces
{
    public interface IAuditService
    {
        void LogTransfer(string fromAccount, string toAccount, decimal amount, string referenceId);
    }
}
