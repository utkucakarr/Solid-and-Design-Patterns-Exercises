namespace FactoryMethod_Vıolation.Notifications
{
    // Somut sınıflar interface yok, kasıtlı olarak kötü
    public class SmsNotification
    {
        public void Send(string recipient, string message)
            => Console.WriteLine($"[SMS] {recipient} -> {message}");
    }
}
