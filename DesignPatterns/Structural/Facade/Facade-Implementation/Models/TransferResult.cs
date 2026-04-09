namespace Facade_Implementation.Models
{
    public class TransferResult
    {
        public bool IsSuccess { get; }
        public string ReferenceId { get; }
        public string FromAccount { get; }
        public string ToAccount { get; }
        public decimal Amount { get; }
        public string Message { get; }

        private TransferResult(
            bool isSuccess,
            string referenceId,
            string fromAccount,
            string toAccount,
            decimal amount,
            string message)
        {
            IsSuccess = isSuccess;
            ReferenceId = referenceId;
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
            Message = message;
        }

        public static TransferResult Success(
            string referenceId, string fromAccount, string toAccount, decimal amount)
            => new(true, referenceId, fromAccount, toAccount, amount,
                $"Transfer başarılı. {fromAccount} → {toAccount} : {amount} TL | Ref: {referenceId}");

        public static TransferResult Fail(
            string reason)
            => new (false, string.Empty, string.Empty, string.Empty, 0,
                $"Transfer başarısız: {reason}");
    }
}
