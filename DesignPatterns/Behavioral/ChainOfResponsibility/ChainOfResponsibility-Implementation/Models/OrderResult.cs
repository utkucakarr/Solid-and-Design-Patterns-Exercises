namespace ChainOfResponsibility_Implementation.Models
{
    public sealed class OrderResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string? TrackingCode { get; }
        public string? FailedHandler { get; }

        private OrderResult(bool isSuccess, string message, string? trackingCode, string? failedHandler)
        {
            IsSuccess = isSuccess;
            Message = message;
            TrackingCode = trackingCode;
            FailedHandler = failedHandler;
        }

        public static OrderResult Success(string trackingCode) =>
            new(true, "Sipariş başarıyla onaylandı.", trackingCode, null);

        public static OrderResult Fail(string reason, string handlerName) =>
            new(false, reason, null, handlerName);
    }
}
