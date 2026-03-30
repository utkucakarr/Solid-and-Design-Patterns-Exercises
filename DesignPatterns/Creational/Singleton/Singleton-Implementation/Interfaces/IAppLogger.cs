using Singleton_Implementation.Enums;

namespace Singleton_Implementation.Interfaces
{
    public interface IAppLogger
    {
        Guid InstanceId { get; }
        void Log(LogLevel level, string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
    }
}
