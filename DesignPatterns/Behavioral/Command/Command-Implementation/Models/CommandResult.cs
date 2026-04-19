namespace Command_Implementation.Models
{
    public sealed class CommandResult
    {
        public bool IsSuccess { get; private init; }
        public string Message { get; private init; } = string.Empty;
        public string DeviceName { get; private init; } = string.Empty;
        public string Action { get; private init; } = string.Empty;

        private CommandResult(){ }

        public static CommandResult Success(
            string message,
            string deviceName,
            string action
            ) =>
            new()
            {
                IsSuccess = true,
                Message = message,
                DeviceName = deviceName,
                Action = action
            };

        public static CommandResult Fail(
            string reason) =>
            new()
            {
                IsSuccess = false,
                Message = reason,
                DeviceName = string.Empty,
                Action = string.Empty,
            };
    }
}