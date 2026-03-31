namespace FactoryMethod_Vıolation.Notifications
{
    public class PushNotification
    {
        public void Send(string recipient, string message)
            => Console.WriteLine($"[PUSH] {recipient} -> {message}");
    }
}
