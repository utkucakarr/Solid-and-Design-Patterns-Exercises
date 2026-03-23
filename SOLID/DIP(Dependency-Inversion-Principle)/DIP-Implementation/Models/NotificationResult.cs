namespace DIP_Implementation.Models
{
    public class NotificationResult
    {
        public bool IsSuccess { get; }
        public string Channel { get; }
        public string Message { get; }

        public NotificationResult(bool isSuccess, string channel, string message)
        {
            IsSuccess = isSuccess;
            Channel = channel;
            Message = message;
        }

        public static NotificationResult Success(string channel, string message)
            => new(true, channel, $"[{channel}] '{message}' gönderildi.");

        public static NotificationResult Fail(string channel, string reason)
            => new(false, channel, $"[{channel}] Gönderilemedi: {reason}");
    }
}
