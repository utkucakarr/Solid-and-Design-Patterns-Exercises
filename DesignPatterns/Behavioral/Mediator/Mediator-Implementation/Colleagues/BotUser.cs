using Mediator_Implementation.Interfaces;
using Mediator_Implementation.Models;

namespace Mediator_Implementation.Colleagues
{
    public class BotUser : IBotUser
    {
        private IChatMediator? _mediator;
        private readonly List<ChatMessage> _messageHistory = new();

        public string Username { get; }
        public UserRole Role => UserRole.Bot;

        public BotUser(string username)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));
            Username = username;
        }

        // Mediator injection — odaya katılınca set edilir
        public void SetMediator(IChatMediator mediator)
        {
            ArgumentNullException.ThrowIfNull(mediator, nameof(mediator));
            _mediator = mediator;
        }

        // Otomatik yanıt — Bot göndereni tanımıyor
        // Mediator üzerinden özel mesaj gönderir
        public ChatResult SendAutoReply(string receiverUsername, string content)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(receiverUsername,
                nameof(receiverUsername));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));
            EnsureMediator();

            return _mediator!.SendPrivateMessage(Username, receiverUsername, content);
        }

        // Mesaj alma — public mesajlara otomatik yanıt verir
        public void ReceiveMessage(ChatMessage message)
        {
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            _messageHistory.Add(message);

            Console.WriteLine($" [{Username}] mesaj aldı -> " +
                $"{message.SenderUsername}: {message.Content}");

            // Yalnızca public mesajlara ve kendi mesajlarına değil yanıt verir
            if (message.Type == MessageType.Public &&
                message.SenderUsername != Username)
            {
                var replyContent =
                    $"Merhaba {message.SenderUsername}! " +
                    $"Mesajınızı aldım: '{message.Content}'";

                // Bot göndereni doğrudan tanımıyor
                // Mediator üzerinden yanıt gönderiyor
                SendAutoReply(message.SenderUsername, replyContent);
            }
        }

        public IReadOnlyList<ChatMessage> GetMessageHistory() =>
            _messageHistory.AsReadOnly();

        private void EnsureMediator()
        {
            if (_mediator is null)
                throw new InvalidOperationException(
                    $"{Username} henüz bir odaya katılmadı.");
        }
    }
}
