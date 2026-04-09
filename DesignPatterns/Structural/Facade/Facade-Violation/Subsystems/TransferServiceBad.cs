namespace Facade_Violation.Subsystems
{
    public class TransferServiceBad
    {
        public string ExecuteTransfer(string fromAccount, string toAccount, decimal amount)
        {
            Console.WriteLine($"[Transfer] {fromAccount} -> {toAccount} : {amount} TL");
            return $"TRF_{Guid.NewGuid():N}";
        }
    }
}
