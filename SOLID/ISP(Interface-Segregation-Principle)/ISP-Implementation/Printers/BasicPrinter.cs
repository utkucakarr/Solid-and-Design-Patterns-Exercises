using ISP_Implementation.Interfaces;
using ISP_Implementation.Models;

namespace ISP_Implementation.Printers
{
    public class BasicPrinter : IPrintable
    {
        public string DeviceName => "Basic Printer";

        public PrinterResult Print(string document)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(document, nameof(document));

            Console.WriteLine($"[PRINT] '{document}' yazdırıldı.");
            return PrinterResult.Success(DeviceName, "PRINT", document);
        }
    }
}
