namespace Mediator_Violation
{
    public class BotUserBad
    {
        public string Username { get; }

        // Bot da diğer kullanıcıları doğrudan tutuyor
        private readonly List<RegularUserBad> _regularUsers = new();
        private readonly List<Message> _messageHistory = new();

        public BotUserBad(string username)
        {
            Username = username;
        }

        public void AddRegularUser(RegularUserBad user) => _regularUsers.Add(user);

        public void ReceiveMessage(Message message)
        {
            _messageHistory.Add(message);

            // Bot yanıtı için doğrudan gönderen kullanıcıya referans gerekiyor
            // Göndereni bulmak için _regularUsers listesinde aramak gerekiyor
            // Gönderen AdminUser veya başka bir BotUser ise yanıt verilemiyor
            var sender = _regularUsers
                .FirstOrDefault(u => u.Username == message.SenderName);

            if (sender != null && !message.IsPrivate)
            {
                var reply = new Message(Username,
                    $"Merhaba {message.SenderName}! Mesajınızı aldım.",
                    isPrivate: true,
                    receiverName: message.SenderName);

                Console.WriteLine($" [{Username}] -> [{sender.Username}]: " +
                    $"Merhaba {message.SenderName}! Mesajınızı aldım.");
                sender.ReceiveMessage(reply);
            }
        }
    }
}
