using Mediator_Implementation.Interfaces;
using Mediator_Implementation.Models;

namespace Mediator_Implementation.Colleagues
{
    public class RegularUser : IRegularUser
    {
        private IChatMediator? _mediator;
        private readonly List<ChatMessage> _messageHistory = new();

        public string Username { get; }
        public UserRole Role => UserRole.Regular;

        public RegularUser(string username)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));
            Username = username;
        }

        // Mediator injection — kullanıcı odaya katıldığında set edilir
        public void SetMediator(IChatMediator mediator)
        {
            ArgumentNullException.ThrowIfNull(mediator, nameof(mediator));
            _mediator = mediator;
        }

        // Mesaj gönderme — doğrudan değil, Mediator üzerinden
        public ChatResult SendMessage(string content)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));
            EnsureMediator();

            return _mediator!.SendMessage(Username, content);
        }

        // Özel mesaj — alıcıyı doğrudan tanımıyor, Mediator bulacak
        public ChatResult SendPrivateMessage(string receiverUsername, string content)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(receiverUsername,
                nameof(receiverUsername));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));
            EnsureMediator();

            return _mediator!.SendPrivateMessage(Username, receiverUsername, content);
        }

        // Odadan ayrılma — Mediator yönetir
        public ChatResult LeaveRoom()
        {
            EnsureMediator();
            _mediator!.Unregister(this);

            return ChatResult.Success(
                message: $"{Username} odadan ayrıldı.",
                senderUsername: Username,
                deliveredCount: 0,
                messageType: MessageType.System
            );
        }

        // Mesaj alma — Mediator tarafından çağrılır
        public void ReceiveMessage(ChatMessage message)
        {
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            _messageHistory.Add(message);

            var prefix = message.Type switch
            {
                MessageType.Private => $"[Özel -> {Username}]",
                MessageType.Broadcast => " [Broadcast]",
                MessageType.System => " [Sistem]",
                _ => $" [{Username}]"
            };

            Console.WriteLine($"  {prefix} {message.SenderUsername}: {message.Content}");
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
