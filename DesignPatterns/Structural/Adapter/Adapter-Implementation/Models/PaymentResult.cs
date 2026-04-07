namespace Adapter_Implementation.Models
{
    public class PaymentResult
    {
        public bool IsSuccess { get; }
        public string TransactionId { get; }
        public string ProviderName { get; }
        public decimal Amount { get; }
        public string Currency { get; }
        public string Message { get; }

        private PaymentResult(
            bool isSucces,
            string transactionId,
            string providerName,
            decimal amount,
            string currency,
            string message)
        {
            IsSuccess = isSucces;
            TransactionId = transactionId;
            ProviderName = providerName;
            Amount = amount;
            Currency = currency;
            Message = message;
        }

        public static PaymentResult Success(
            string transactionId,
            string providerName,
            decimal amount,
            string currency)
            => new(true, transactionId, providerName, amount, currency,
               $"[{providerName}] {amount} {currency} başarıyla ödendi.");

        public static PaymentResult Fail(string providerName, string reason)
            => new (false, string.Empty, providerName, 0, string.Empty,
                $"[{providerName}] Ödeme başarısız: {reason}");
    }
}
