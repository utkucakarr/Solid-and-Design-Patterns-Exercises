namespace DIP_Violation
{
    public class PushService
    {
        public void Send(string message)
            => Console.WriteLine($"[SMS] {message}");
    }
}
