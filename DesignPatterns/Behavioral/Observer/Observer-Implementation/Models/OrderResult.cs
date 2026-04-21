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

        public static OrderResult Success(
        string orderId,
        OrderStatus newStatus,
        int notifiedObserverCount) =>
        new(
            isSuccess: true,
            message: $"Sipariş {orderId} durumu '{newStatus}' olarak güncellendi. " +
                     $"{notifiedObserverCount} observer bilgilendirildi.",
            orderId: orderId,
            newStatus: newStatus,
            notifiedObserverCount: notifiedObserverCount
        );

        public static OrderResult Fail(string reason) =>
            new(
                isSuccess: false,
                message: reason,
                orderId: null,
                newStatus: null,
                notifiedObserverCount: 0
            );
    }
}
