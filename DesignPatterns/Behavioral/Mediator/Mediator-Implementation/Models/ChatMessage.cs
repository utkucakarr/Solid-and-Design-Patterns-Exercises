namespace Mediator_Implementation.Models
{
    public sealed class ChatMessage
    {
        public string SenderUsername { get; }
        public string Content { get; }
        public MessageType Type { get; }
        public string? ReceiverUsername { get; }
        public DateTime SentAt { get; }

        private ChatMessage(
            string senderUsername,
            string content,
            MessageType type,
            string? receiverUsername)
        {
            SenderUsername = senderUsername;
            Content = content;
            Type = type;
            ReceiverUsername = receiverUsername;
            SentAt = DateTime.UtcNow;
        }

        // Static factory methods — her mesaj tipi için ayrı oluşturucu
        public static ChatMessage Public(string senderUsername, string content) =>
            new(senderUsername, content, MessageType.Public, null);

        public static ChatMessage Private(
            string senderUsername,
            string receiverUsername,
            string content) =>
            new(senderUsername, content, MessageType.Private, receiverUsername);

        public static ChatMessage Broadcast(string senderUsername, string content) =>
            new(senderUsername, content, MessageType.Broadcast, null);

        public static ChatMessage System(string content) =>
            new("SYSTEM", content, MessageType.System, null);
    }
}
