namespace Decorator_Violation.Email
{
    public class EmailServiceBad
    {
        public class EmailMessageBad
        {
            public string To { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }

        // Her özellik kombinasyonu için ayrı sınıf yazılıyor - class explosion!
        public class BasicEmailServiceBad
        {
            public bool Send(EmailMessageBad message)
            {
                Console.WriteLine($"[Email] {message.To} → {message.Subject}");
                return true;
            }
        }

        // Sıkıştırma istiyorsak ayrı bir sınıf
        public class CompressedEmailServiceBad
        {
            public bool Send(EmailMessageBad message)
            {
                // Gönderim kodu burada da tekrar yazıldı!
                Console.WriteLine($"[Sıkıştırma] Body sıkıştırıldı.");
                Console.WriteLine($"[Email] {message.To} → {message.Subject}");
                return true;
            }
        }

        // Şifreleme istiyorsan ayrı sınıf
        public class EncryptedEmailService_Bad
        {
            public bool Send(EmailMessageBad message)
            {
                // Gönderim kodu burada da tekrar yazıldı!
                Console.WriteLine($"[Şifreleme] Body şifrelendi.");
                Console.WriteLine($"[Email] {message.To} → {message.Subject}");
                return true;
            }
        }

        // Sıkıştırma + Şifreleme istiyorsan ayrı sınıf
        public class CompressedEncryptedEmailService_Bad
        {
            public bool Send(EmailMessageBad message)
            {
                // Gönderim kodu 3. kez tekrar yazıldı!
                Console.WriteLine($"[Sıkıştırma] Body sıkıştırıldı.");
                Console.WriteLine($"[Şifreleme] Body şifrelendi.");
                Console.WriteLine($"[Email] {message.To} → {message.Subject}");
                return true;
            }
        }

        // Sıkıştırma + Şifreleme + Loglama istiyorsan YET BAŞKA sınıf
        public class CompressedEncryptedLoggedEmailService_Bad
        {
            public bool Send(EmailMessageBad message)
            {
                // Gönderim kodu 4. kez tekrar yazıldı!
                Console.WriteLine($"[Log] Gönderim başlıyor. To: {message.To}");
                Console.WriteLine($"[Sıkıştırma] Body sıkıştırıldı.");
                Console.WriteLine($"[Şifreleme] Body şifrelendi.");
                Console.WriteLine($"[Email] {message.To} → {message.Subject}");
                Console.WriteLine($"[Log] Gönderim tamamlandı.");
                return true;
            }
        }
        // 3 özellik = 2^3 = 8 farklı kombinasyon = 8 ayrı sınıf!
        // 4 özellik = 2^4 = 16 farklı kombinasyon — class explosion!
        // Yeni özellik (retry) = mevcut tüm kombinasyonlara dokun!
    }
}