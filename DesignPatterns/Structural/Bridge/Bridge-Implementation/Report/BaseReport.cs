using Bridge_Implementation.Interfaces;
using Bridge_Implementation.Models;

namespace Bridge_Implementation.Report
{
    // RefinedAbstraction base — renderer bridge üzerinden tutulur
    public abstract class BaseReport : IReport
    {

        // Bridge - Abstraction, Implementation'ı bu referans üzerinden kullanıyor
        protected readonly IReportRenderer Renderer;

        public abstract string ReportName { get; }

        protected BaseReport(IReportRenderer renderer)
        {
            Renderer = renderer
                ?? throw new ArgumentNullException(nameof(renderer));
        }

        public abstract ReportResult Generate();

        // Alt sınıfların ortak kullandığı render yardımcısı
        protected ReportResult Render(string content, Dictionary<string, string> metadata)
        {
            try
            {
                var rendered = Renderer.Render(ReportName, content, metadata);
                return ReportResult.Success(ReportName, Renderer.RenderName, rendered);
            }
            catch (Exception ex)
            {
                return ReportResult.Fail(ReportName, ex.Message);
            }
        }
    }
}
