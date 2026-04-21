using TemplateMethod_Implementation.Models;

namespace TemplateMethod_Implementation.Interfaces
{
    public interface IReportGenerator
    {
        ReportResult Generate(string reportTitle, IEnumerable<string> data);
    }
}
