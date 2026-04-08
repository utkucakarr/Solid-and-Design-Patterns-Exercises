namespace Singleton_Violation.Logging
{
    public class Logger
    {
        // Her servis kendi Logger'ını oluşturuyor
        private readonly string _logFilePath;
        public Guid InstanceId { get; } = Guid.NewGuid();

        //  Her new'lemede yeni instance — aynı dosyaya birden fazla
        //  yazıcı erişebilir, çakışma ve veri kaybı riski!
        public Logger(string logFilePatch)
        {
            _logFilePath = logFilePatch;
            Console.WriteLine($"[Logger] Yeni instance oluşturuldu. ID: {InstanceId}");
        }

        public void Log(string message)
        {
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            Console.WriteLine(logEntry);
        }
    }
}
