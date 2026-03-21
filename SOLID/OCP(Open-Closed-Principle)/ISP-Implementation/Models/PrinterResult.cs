namespace ISP_Implementation.Models
{
    public class PrinterResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string DeviceName { get;}

        public PrinterResult(bool isSuccess, string message, string deviceName)
        {
            IsSuccess = isSuccess;
            Message = message;
            DeviceName = deviceName;
        }

        public static PrinterResult Success(string deviceName, string operation, string document)
            => new(true, $"[{operation}] '{document}' işlemi başarılı.", deviceName);

        public static PrinterResult Fail(string deviceName, string reason)
            => new(false, $"'{deviceName}' işlem başarısız: {reason}", deviceName);
    }
}
