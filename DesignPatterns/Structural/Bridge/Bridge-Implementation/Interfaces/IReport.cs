using Bridge_Implementation.Models;

namespace Bridge_Implementation.Interfaces
{
    // Abstaction - rapor hiyerarşisinin sözleşmesi
    public interface IReport
    {
        string ReportName { get; }
        ReportResult Generate();
    }
}
