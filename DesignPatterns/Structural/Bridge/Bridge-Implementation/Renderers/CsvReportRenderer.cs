using Bridge_Implementation.Interfaces;

namespace Bridge_Implementation.Renderers
{
    // ConcreteImplementor — CSV format
    public class CsvReportRenderer : IReportRenderer
    {
        public string RenderName => "CSV";

        public string Render(string title, string content, Dictionary<string, string> metadata)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));


            Console.WriteLine($"[CSV Renderer] '{title}' raporu CSV olarak render ediliyor.");

            var headers = string.Join(",", metadata.Keys);
            var values = string.Join(",", metadata.Values);

            return $"# {title}\n{headers}\n{values}\n{content}";
        }
    }
}
