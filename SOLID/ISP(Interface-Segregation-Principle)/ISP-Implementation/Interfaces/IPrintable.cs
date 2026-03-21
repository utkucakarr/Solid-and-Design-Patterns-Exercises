using ISP_Implementation.Models;

namespace ISP_Implementation.Interfaces
{
    public interface IPrintable
    {
        PrinterResult Print(string document);
    }
}
