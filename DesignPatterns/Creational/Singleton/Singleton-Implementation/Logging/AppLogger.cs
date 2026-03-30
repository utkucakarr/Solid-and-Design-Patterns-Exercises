using Singleton_Implementation.Enums;
using Singleton_Implementation.Interfaces;

namespace Singleton_Implementation.Logging
{
    public sealed class AppLogger : IAppLogger
    {
        // Tek instance burada tutuluyor
        private static AppLogger? _instance;

        // Thread-safe için lock nesnesi
        private static readonly object _lock = new();

        // Dosyaya eş zamanlı yazma için ayrı lock
        private static readonly object _fileLock = new();

        private readonly string _logFilePath;

        public Guid InstanceId { get; } = Guid.NewGuid();

        // Private constructor dışarıdan new'lenemez!
        private AppLogger()
        {
            var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDirectory);
            _logFilePath = Path.Combine(logDirectory, $"app-{DateTime.Now:yyyy-MM-dd}.log");

            Console.WriteLine($"[AppLogger] Singleton instance oluşturuldu. ID: {InstanceId}");
            Console.WriteLine($"[AppLogger] Log dosyası: {_logFilePath}");
        }

        // Thread-safe Singleton - double-checked locking
        public static AppLogger GetInstance()
        {
            if(_instance is null)
            {
                lock (_lock)
                {
                    if (_instance is null)
                        _instance = new AppLogger();
                }
            }
            return _instance;
        }

        public void Log(LogLevel level, string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
               $"[{level.ToString().ToUpper()}] " +
               $"[Instance: {InstanceId}] " +
               $"{message}";

            // Thread-safe dosya yazma
            lock (_fileLock)
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }

            var color = level switch
            {
                LogLevel.Info => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = color;
            Console.WriteLine(logEntry);
            Console.ResetColor();
        }

        public void Info(string message) => Log(LogLevel.Info, message);

        public void Warning(string message) => Log(LogLevel.Warning, message);

        public void Error(string message) => Log(LogLevel.Error, message);

    }
}