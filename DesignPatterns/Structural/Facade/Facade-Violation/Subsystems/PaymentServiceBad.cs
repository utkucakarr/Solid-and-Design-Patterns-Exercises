namespace Facade_Violation.Subsystems
{
    public class PaymentServiceBad
    {
        public string? ProcessPayment(decimal amount, string currency)
        {
            Console.WriteLine($"[Ödeme] {amount} {currency} tahsil ediliyor.");
            return $"TXN_{Guid.NewGuid():N}";
        }

        public void Refund(string transactionId)
            => Console.WriteLine($"[Ödeme] {transactionId} iade ediliyor.");
    }
}
