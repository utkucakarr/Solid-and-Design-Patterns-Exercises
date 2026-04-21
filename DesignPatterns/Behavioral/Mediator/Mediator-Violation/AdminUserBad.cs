namespace Mediator_Violation
{
    public class AdminUserBad
    {
        public string Username { get; }

        // Admin da tüm kullanıcıları doğrudan referans tutuyor
        private readonly List<RegularUserBad> _regularUsers = new();
        private readonly List<AdminUserBad> _otherAdmins = new();
        private readonly List<BotUserBad> _bots = new();
        private readonly List<Message> _messageHistory = new();

        public AdminUserBad(string username)
        {
            Username = username;
        }

        public void AddRegularUser(RegularUserBad user) => _regularUsers.Add(user);
        public void AddAdmin(AdminUserBad admin) => _otherAdmins.Add(admin);
        public void AddBot(BotUserBad bot) => _bots.Add(bot);

        public void Broadcast(string content)
        {
            // Broadcast için yine tüm listeleri ayrı iterate etmek gerekiyor
            var message = new Message(Username, $"[SİSTEM] {content}");
            _messageHistory.Add(message);

            Console.WriteLine($"[ADMIN:{Username}] -> Broadcast: {content}");

            foreach (var user in _regularUsers)
                user.ReceiveMessage(message);

            foreach (var admin in _otherAdmins)
                admin.ReceiveMessage(message);

            foreach (var bot in _bots)
                bot.ReceiveMessage(message);
        }

        public void KickUser(RegularUserBad user)
        {
            // Kick işlemi için doğrudan kullanıcı referansı gerekiyor
            // Diğer kullanıcılara bildirim için tüm listeler tekrar iterate ediliyor
            _regularUsers.Remove(user);
            Console.WriteLine($"[ADMIN:{Username}] -> {user.Username} odadan atıldı.");

            foreach (var u in _regularUsers)
                Console.WriteLine($" [{u.Username}]: {user.Username} odadan atıldı.");
        }

        public void ReceiveMessage(Message message)
        {
            _messageHistory.Add(message);
            Console.WriteLine($"  📨 [ADMIN:{Username}] mesaj aldı: {message.Content}");
        }
    }
}
