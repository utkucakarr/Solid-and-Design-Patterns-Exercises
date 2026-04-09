using Facade_Implementation.Interfaces;

namespace Facade_Implementation.Subsystems
{
    public class TransferService : ITransferService
    {
        public string ExecuteTransfer(string fromAccount, string toAccount, decimal amount)
        {
            Console.WriteLine($"[Transfer] {fromAccount} -> {toAccount} : {amount} TL");
            return $"TRF_{Guid.NewGuid():N}";
        }
    }
}
