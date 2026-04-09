namespace Facade_Implementation.Interfaces
{
    public interface ITransferService
    {
        string ExecuteTransfer(string fromAccount, string toAccount, decimal amount);
    }
}
