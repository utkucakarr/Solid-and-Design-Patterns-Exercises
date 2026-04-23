namespace State_Implementation.Models
{
    public class OrderResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string OrderId { get; }
        public string FromState { get; }
        public string ToState { get; }

        private OrderResult(
            bool isSuccess,
            string message,
            string orderId,
            string fromState,
            string toState)
        {
            IsSuccess = isSuccess;
            Message = message;
            OrderId = orderId;
            FromState = fromState;
            ToState = toState;
        }

        // Static factory: başarılı geçiş
        public static OrderResult Success(string orderId, string message, string fromState, string toState)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(orderId, nameof(orderId));
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));
            ArgumentException.ThrowIfNullOrWhiteSpace(fromState, nameof(fromState));
            ArgumentException.ThrowIfNullOrWhiteSpace(toState, nameof(toState));

            return new OrderResult(true, message, orderId, fromState, toState);
        }

        // Static factory: geçersiz geçiş
        public static OrderResult Fail(string orderId, string reason, string currentState)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(orderId, nameof(orderId));
            ArgumentException.ThrowIfNullOrWhiteSpace(reason, nameof(reason));
            ArgumentException.ThrowIfNullOrWhiteSpace(currentState, nameof(currentState));

            return new OrderResult(false, reason, orderId, currentState, currentState);
        }
    }
}
