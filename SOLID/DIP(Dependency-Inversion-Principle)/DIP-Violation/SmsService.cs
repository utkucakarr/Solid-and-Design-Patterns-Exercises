using System.Threading.Channels;

namespace DIP_Violation
{
    public class SmsService
    {
        public void Send(string message)
            => Console.WriteLine($"[SMS] {message}");
    }
}
