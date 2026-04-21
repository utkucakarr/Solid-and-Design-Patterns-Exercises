namespace Mediator_Implementation.Models
{
    public sealed class ChatResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string? SenderUsername { get; }
        public string? ReceiverUsername { get; }
        public int DeliveredCount { get; }
        public MessageType? MessageType { get; }

        private ChatResult(
            bool isSuccess,
            string message,
            string? senderUsername,
            string? receiverUsername,
            int deliveredCount,
            MessageType? messageType)
        {
            IsSuccess = isSuccess;
            Message = message;
            SenderUsername = senderUsername;
            ReceiverUsername = receiverUsername;
            DeliveredCount = deliveredCount;
            MessageType = messageType;
        }

        public static ChatResult Success(
            string message,
            string senderUsername,
            int deliveredCount,
            MessageType messageType,
            string? receiverUsername = null) =>
            new(
                isSuccess: true,
                message: message,
                senderUsername: senderUsername,
                receiverUsername: receiverUsername,
                deliveredCount: deliveredCount,
                messageType: messageType
            );

        public static ChatResult Fail(string reason) =>
            new(
                isSuccess: false,
                message: reason,
                senderUsername: null,
                receiverUsername: null,
                deliveredCount: 0,
                messageType: null
            );
    }
}
