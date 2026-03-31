namespace FactoryMethod_Implementation.Models
{
    public class NotificationResult
    {
        public bool IsSuccess { get; }
        public string Channel { get; }
        public string Recipient { get; }
        public string Message { get; }

        private NotificationResult(bool isSuccess, string channel, string recipient, string message)
        {
            IsSuccess = isSuccess;
            Channel = channel;
            Recipient = recipient;
            Message = message;
        }

        public static NotificationResult Success(string channel, string recipient, string message)
            => new(true, channel, recipient,
                $"[{channel}] '{message}' -> '{recipient}' başarıyla gönderildi.");

        public static NotificationResult Fail(string channel, string recipient, string reason)
            => new(false, channel, recipient,
                $"[{channel}] '{recipient}' gönderilemedi: {reason}");
    }
}
