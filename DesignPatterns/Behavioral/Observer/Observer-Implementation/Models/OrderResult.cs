namespace Observer_Implementation.Models
{
    public sealed class OrderResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string? OrderId { get; }
        public OrderStatus? NewStatus { get; }
        public int NotifiedObserverCount { get; }

        private OrderResult(
       bool isSuccess,
       string message,
       string? orderId,
       OrderStatus? newStatus,
       int notifiedObserverCount)
        {
            IsSuccess = isSuccess;
            Message = message;
            OrderId = orderId;
            NewStatus = newStatus;
            NotifiedObserverCount = notifiedObserverCount;
        }
    }
}
