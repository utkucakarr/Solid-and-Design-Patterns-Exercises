using ISP_Implementation.Models;

namespace ISP_Implementation.Interfaces
{
    public interface IFaxable
    {
        PrinterResult Fax(string document);
    }
}
