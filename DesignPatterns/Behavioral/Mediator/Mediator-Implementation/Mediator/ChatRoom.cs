using Mediator_Implementation.Interfaces;
using Mediator_Implementation.Models;

namespace Mediator_Implementation.Mediator
{
    public class ChatRoom : IChatMediator
    {
        private readonly string _roomName;
        private readonly Dictionary<string, IUser> _users = new();

        public ChatRoom(string roomName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(roomName, nameof(roomName));
            _roomName = roomName;
        }

        // Register — kullanıcıyı odaya kaydeder ve Mediator'ı set eder
        public void Register(IUser user)
        {
            ArgumentNullException.ThrowIfNull(user, nameof(user));
            ArgumentException.ThrowIfNullOrWhiteSpace(user.Username, nameof(user.Username));

            if (_users.ContainsKey(user.Username))
                return;

            // Colleague sadece Mediator'ı tanır — diğer kullanıcıları bilmez
            user.SetMediator(this);
            _users[user.Username] = user;

            // Katılım bildirimi — tüm kullanıcılara sistem mesajı
            var systemMessage = ChatMessage.System(
                $"{user.Username} odaya katıldı. " +
                $"({_users.Count} kullanıcı aktif)");

            NotifyAll(systemMessage, excludeUsername: null);
            Console.WriteLine($" [Sistem] {user.Username} '{_roomName}' odasına katıldı.");
        }

        // Unregister — kullanıcıyı odadan çıkarır
        public void Unregister(IUser user)
        {
            ArgumentNullException.ThrowIfNull(user, nameof(user));

            if (!_users.ContainsKey(user.Username))
                return;

            _users.Remove(user.Username);

            // Ayrılma bildirimi — kalan kullanıcılara sistem mesajı
            var systemMessage = ChatMessage.System(
                $"{user.Username} odadan ayrıldı. " +
                $"({_users.Count} kullanıcı aktif)");

            NotifyAll(systemMessage, excludeUsername: null);
            Console.WriteLine($" [Sistem] {user.Username} '{_roomName}' odasından ayrıldı.");
        }

        // SendMessage — herkese public mesaj iletir
        public ChatResult SendMessage(string senderUsername, string content)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(senderUsername, nameof(senderUsername));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

            if (!_users.TryGetValue(senderUsername, out var sender))
                return ChatResult.Fail($"'{senderUsername}' kullanıcısı odada bulunamadı.");

            var message = ChatMessage.Public(senderUsername, content);

            // Gönderen hariç herkese iletilir
            var deliveredCount = NotifyAll(message, excludeUsername: senderUsername);

            return ChatResult.Success(
                message: $"Mesaj {deliveredCount} kullanıcıya iletildi.",
                senderUsername: senderUsername,
                deliveredCount: deliveredCount,
                messageType: MessageType.Public
            );
        }

        // SendPrivateMessage — yalnızca alıcıya iletir
        public ChatResult SendPrivateMessage(
            string senderUsername,
            string receiverUsername,
            string content)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(senderUsername, nameof(senderUsername));
            ArgumentException.ThrowIfNullOrWhiteSpace(receiverUsername, nameof(receiverUsername));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

            if (!_users.ContainsKey(senderUsername))
                return ChatResult.Fail($"'{senderUsername}' kullanıcısı odada bulunamadı.");

            if (!_users.TryGetValue(receiverUsername, out var receiver))
                return ChatResult.Fail($"'{receiverUsername}' alıcısı odada bulunamadı.");

            var message = ChatMessage.Private(senderUsername, receiverUsername, content);

            // Yalnızca alıcıya iletilir — Mediator yönlendirmeyi bilir
            receiver.ReceiveMessage(message);

            return ChatResult.Success(
                message: $"Özel mesaj '{receiverUsername}' kullanıcısına iletildi.",
                senderUsername: senderUsername,
                receiverUsername: receiverUsername,
                deliveredCount: 1,
                messageType: MessageType.Private
            );
        }

        // Broadcast — yalnızca Admin rolüne izin verilir
        public ChatResult Broadcast(string senderUsername, string content)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(senderUsername, nameof(senderUsername));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

            if (!_users.TryGetValue(senderUsername, out var sender))
                return ChatResult.Fail($"'{senderUsername}' kullanıcısı odada bulunamadı.");

            // Yetki kontrolü Mediator'da — Colleague bilmez
            if (sender.Role != UserRole.Admin)
                return ChatResult.Fail(
                    $"'{senderUsername}' broadcast yetkisine sahip değil. " +
                    $"Yalnızca Admin kullanıcılar broadcast yapabilir.");

            var message = ChatMessage.Broadcast(senderUsername, $"[DUYURU] {content}");

            var deliveredCount = NotifyAll(message, excludeUsername: null);

            return ChatResult.Success(
                message: $"Broadcast {deliveredCount} kullanıcıya iletildi.",
                senderUsername: senderUsername,
                deliveredCount: deliveredCount,
                messageType: MessageType.Broadcast
            );
        }

        // KickUser — yalnızca Admin rolüne izin verilir
        public ChatResult KickUser(string adminUsername, string targetUsername)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(adminUsername, nameof(adminUsername));
            ArgumentException.ThrowIfNullOrWhiteSpace(targetUsername, nameof(targetUsername));

            if (!_users.TryGetValue(adminUsername, out var admin))
                return ChatResult.Fail($"'{adminUsername}' kullanıcısı odada bulunamadı.");

            // Yetki kontrolü Mediator'da
            if (admin.Role != UserRole.Admin)
                return ChatResult.Fail(
                    $"'{adminUsername}' kick yetkisine sahip değil. " +
                    $"Yalnızca Admin kullanıcılar kick yapabilir.");

            if (!_users.TryGetValue(targetUsername, out var target))
                return ChatResult.Fail($"'{targetUsername}' kullanıcısı odada bulunamadı.");

            // Admin kendini kick edemez
            if (adminUsername == targetUsername)
                return ChatResult.Fail("Kendinizi odadan atamazsınız.");

            _users.Remove(targetUsername);

            // Tüm kullanıcılara bildirim
            var systemMessage = ChatMessage.System(
                $"{targetUsername} admin tarafından odadan atıldı.");

            NotifyAll(systemMessage, excludeUsername: null);

            Console.WriteLine($" [Sistem] {targetUsername} " +
                $"'{adminUsername}' tarafından odadan atıldı.");

            return ChatResult.Success(
                message: $"'{targetUsername}' odadan atıldı.",
                senderUsername: adminUsername,
                receiverUsername: targetUsername,
                deliveredCount: _users.Count,
                messageType: MessageType.System
            );
        }

        // Aktif kullanıcı listesi
        public IReadOnlyList<IUser> GetActiveUsers() =>
            _users.Values.ToList().AsReadOnly();

        // NotifyAll — tüm kullanıcılara mesaj iletir
        // excludeUsername null ise herkese gönderilir
        private int NotifyAll(ChatMessage message, string? excludeUsername)
        {
            var recipients = _users.Values
                .Where(u => u.Username != excludeUsername)
                .ToList();

            foreach (var user in recipients)
                user.ReceiveMessage(message);

            return recipients.Count;
        }
    }
}
