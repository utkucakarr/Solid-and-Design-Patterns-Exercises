using SingleResponsibility.Interfaces;

namespace SingleResponsibility.Services
{
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            Console.WriteLine($"[BİLGİ - {DateTime.Now:HH:mm}]: {message}");
        }
    }
}