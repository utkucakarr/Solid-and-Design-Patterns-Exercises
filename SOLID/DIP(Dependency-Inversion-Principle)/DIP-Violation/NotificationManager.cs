namespace DIP_Violation
{
    // DIP İHLALİ — Üst seviye modül, alt seviye somut sınıflara bağımlı
    public class NotificationManager
    {
        // Somut sınıfları doğrudan new'liyor — sıkı bağımlılık!
        private readonly EmailService _emailService = new();
        private readonly SmsService _smsService = new();
        private readonly PushService _pushService = new();

        // Yeni bir kanal eklemek için bu sınıfın içine girmek zorundayız!
        public void SendAll(string message)
        {
            _emailService.Send(message);
            _smsService.Send(message);
            _pushService.Send(message);
        }

        public void SendEmail(string message) => _emailService.Send(message);
        public void SendSms(string message) => _smsService.Send(message);
        public void SendPush(string message) => _pushService.Send(message);
    }
}
