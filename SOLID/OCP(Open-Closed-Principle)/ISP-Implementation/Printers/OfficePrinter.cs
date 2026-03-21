using ISP_Implementation.Interfaces;
using ISP_Implementation.Models;

namespace ISP_Implementation.Printers
{
    public class OfficePrinter : IPrintable, IScannable
    {
        public string DeviceName => "Office Printer";

        public PrinterResult Print(string document)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(document, nameof(document));

            Console.WriteLine($"[PRINT] '{document}' yazdırıldı.");
            return PrinterResult.Success(DeviceName, "PRINT", document);
        }

        public PrinterResult Scan(string document)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(document, nameof(document));

            Console.WriteLine($"[SCAN] '{document}' tarandı.");
            return PrinterResult.Success(DeviceName, "SCAN", document);
        }
    }
}
