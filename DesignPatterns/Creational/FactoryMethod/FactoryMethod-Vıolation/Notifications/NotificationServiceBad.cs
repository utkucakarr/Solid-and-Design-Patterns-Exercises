namespace FactoryMethod_Vıolation.Notifications
{
    public class NotificationServiceBad
    {
        //  Factory Method ihlali — nesne oluşturma sorumluluğu
        //  client'a yüklenmiş, if-else zincirleri var
        public void Send(string type, string recipient, string message)
        {
            var typeLower = type.ToLower();
            if (typeLower == "email")
            {
                var emailNotification = new EmailNotificaion();
                emailNotification.Send(recipient, message);
            }

            else if (typeLower == "sms")
            {
                var smsNotification = new SmsNotification();
                smsNotification.Send(recipient, message);
            }
            else if (typeLower == "push")
            {
                var pushNotification = new PushNotification();
                pushNotification.Send(recipient, message);
            }
            else
            {
                throw new ArgumentException($"Geçersiz bildirim tipi: {typeLower}");
            }
        }
    }
}
