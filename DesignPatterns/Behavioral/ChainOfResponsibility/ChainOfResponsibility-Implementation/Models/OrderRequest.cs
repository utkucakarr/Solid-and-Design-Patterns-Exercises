namespace ChainOfResponsibility_Implementation.Models
{
    public sealed class OrderRequest
    {
        public string ProductId { get; }
        public string CustomerId { get; }
        public int Quantity { get; }
        public decimal AccountBalance { get; }

        public OrderRequest(string productId, string customerId, int quantity, decimal accountBalance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(productId, nameof(productId));
            ArgumentException.ThrowIfNullOrWhiteSpace(customerId, nameof(customerId));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
            ArgumentOutOfRangeException.ThrowIfNegative(accountBalance, nameof(accountBalance));

            ProductId = productId;
            CustomerId = customerId;
            Quantity = quantity;
            AccountBalance = accountBalance;
        }
    }
}
