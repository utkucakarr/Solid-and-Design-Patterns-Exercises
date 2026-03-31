namespace FactoryMethod_Vıolation.Notifications
{
    public class EmailNotificaion
    {
        public void Send(string recipient, string message)
            => Console.WriteLine($"[EMAİL] {recipient} -> {message}");
    }
}
