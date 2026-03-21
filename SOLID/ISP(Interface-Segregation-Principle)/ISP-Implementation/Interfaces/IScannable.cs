using ISP_Implementation.Models;

namespace ISP_Implementation.Interfaces
{
    public interface IScannable
    {
        PrinterResult Scan(string document);
    }
}
