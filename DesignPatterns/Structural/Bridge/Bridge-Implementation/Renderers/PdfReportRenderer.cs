using Bridge_Implementation.Interfaces;

namespace Bridge_Implementation.Renderers
{
    // ConcreteImplementor - PDF Format
    public class PdfReportRenderer : IReportRenderer
    {
        public string RenderName => "PDF";

        public string Render(string title, string content, Dictionary<string, string> metadata)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

            var metaSection = string.Join(" | ", metadata.Select(m => $"{m.Key}: {m.Value}"));
            Console.WriteLine($"[PDF Renderer] '{title}' raporu PDF olarak render ediliyor.");

            return $"[PDF_START]" +
                $"[TITLE]{title}[/TITLE]" +
                $"[META]{metaSection}[/META]" +
                $"[BODY]{content}[/BODY]" +
                $"[PDF_END]";
        }
    }
}
