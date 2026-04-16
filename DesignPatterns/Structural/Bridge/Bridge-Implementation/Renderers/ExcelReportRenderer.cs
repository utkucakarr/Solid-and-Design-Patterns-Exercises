using Bridge_Implementation.Interfaces;

namespace Bridge_Implementation.Renderers
{
    // ConcreteImplementor — Excel format
    public class ExcelReportRenderer : IReportRenderer
    {
        public string RenderName => "Excel";

        public string Render(string title, string content, Dictionary<string, string> metadata)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

            Console.WriteLine($"[Excel Renderer] '{title}' raporu Excel olarak render ediliyor.");

            var rows = metadata.Select(m => $"<row><cell>{m.Key}</cell><cell>{m.Value}</cell></row>");
            return $"<workbook>" +
                   $"<sheet name='{title}'>" +
                   $"<header>{title}</header>" +
                   string.Join("", rows) +
                   $"<data>{content}</data>" +
                   $"</sheet></workbook>";
        }
    }
}
