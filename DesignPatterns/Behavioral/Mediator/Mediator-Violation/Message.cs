namespace Mediator_Violation
{
    public class Message
    {
        // Message sınıfı tamamen dışarıya açık — encapsulation yok
        public string SenderName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsPrivate { get; set; }
        public string? ReceiverName { get; set; }

        public Message(string senderName, string content,
            bool isPrivate = false, string? receiverName = null)
        {
            SenderName = senderName;
            Content = content;
            SentAt = DateTime.UtcNow;
            IsPrivate = isPrivate;
            ReceiverName = receiverName;
        }
    }
}
