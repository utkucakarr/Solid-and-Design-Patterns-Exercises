namespace Mediator_Violation
{
    public class RegularUserBad
    {
        public string Username { get; }

        // Her kullanıcı diğer tüm kullanıcıları doğrudan referans tutuyor
        // Many-to-many bağımlılık — N kullanıcı için N*(N-1) bağımlılık oluşuyor
        private readonly List<RegularUserBad> _contacts = new();
        private readonly List<AdminUserBad> _adminContacts = new();
        private readonly List<BotUserBad> _botContacts = new();

        // Mesaj geçmişi her kullanıcıda ayrı tutuluyor — senkronizasyon sorunu
        private readonly List<Message> _messageHistory = new();

        public RegularUserBad(string username)
        {
            // Guard clause yok
            Username = username;
        }

        // Kullanıcı eklemek için her tip için ayrı metot gerekiyor
        public void AddContact(RegularUserBad user) => _contacts.Add(user);
        public void AddAdminContact(AdminUserBad admin) => _adminContacts.Add(admin);
        public void AddBotContact(BotUserBad bot) => _botContacts.Add(bot);

        public void SendMessage(string content)
        {
            // Mesaj göndermek için tüm listeleri ayrı ayrı iterate etmek gerekiyor
            // Yeni kullanıcı tipi eklenirse bu metot değiştirilmeli — OCP ihlali
            var message = new Message(Username, content);
            _messageHistory.Add(message);

            Console.WriteLine($"[{Username}] → Herkese: {content}");

            foreach (var contact in _contacts)
            {
                // Doğrudan başka kullanıcının metodunu çağırıyor — sıkı bağımlılık
                contact.ReceiveMessage(message);
            }

            foreach (var admin in _adminContacts)
            {
                // Her tip için tekrar eden kod — DRY ihlali
                admin.ReceiveMessage(message);
            }

            foreach (var bot in _botContacts)
            {
                // Bot tipi değişirse bu kod da değişmeli
                bot.ReceiveMessage(message);
            }
        }

        public void SendPrivateMessage(string content, RegularUserBad receiver)
        {
            // Özel mesaj için doğrudan alıcıya bağımlılık
            // AdminUser veya BotUser'a özel mesaj göndermek için
            //    ayrı overload yazmak gerekiyor
            var message = new Message(Username, content,
                isPrivate: true, receiverName: receiver.Username);

            Console.WriteLine($"[{Username}] → [{receiver.Username}] (özel): {content}");
            receiver.ReceiveMessage(message);
        }

        public void ReceiveMessage(Message message)
        {
            _messageHistory.Add(message);

            if (message.IsPrivate)
                Console.WriteLine($" [{Username}] özel mesaj aldı: {message.Content}");
            else
                Console.WriteLine($" [{Username}] mesaj aldı: {message.Content}");
        }

        // Odadan ayrılmak için tüm kullanıcıların listelerinden
        //    bu kullanıcıyı manuel olarak çıkarmak gerekiyor
        public void NotifyLeaving()
        {
            foreach (var contact in _contacts)
                Console.WriteLine($" [{contact.Username}]: {Username} odadan ayrıldı.");

            foreach (var admin in _adminContacts)
                Console.WriteLine($" [{admin.Username}]: {Username} odadan ayrıldı.");
        }

        public IReadOnlyList<Message> GetHistory() => _messageHistory.AsReadOnly();
    }
}
