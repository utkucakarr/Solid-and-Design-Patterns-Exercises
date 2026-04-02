using Prototype_Implementation.Interfaces;
using Prototype_Implementation.Models;

namespace Prototype_Implementation.Documents
{
    public class ReportDocument : IDocumentPrototype<ReportDocument>
    {
        public string DocumentType => "Report";
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> TableData { get; set; }
        public DocumentMetadata Metadata { get; set; }

        public ReportDocument(
            string title,
            string content,
            List<string> tableData,
            DocumentMetadata metadata)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentNullException.ThrowIfNull(tableData, nameof(tableData));
            ArgumentNullException.ThrowIfNull(metadata, nameof(metadata));


            Title = title;
            Content = content;
            TableData = tableData;
            Metadata = metadata;

            Console.WriteLine($"[ReportDocument] '{title}' oluşturuldu.");
        }

        // Sığ kopya — TableData listesi paylaşılıyor
        public ReportDocument Clone()
        {
            var clone = (ReportDocument)MemberwiseClone();
            Console.WriteLine($"[ReportDocument] '{Title}' sığ kopyalandı.");
            return clone;
        }

        // Derin kopya — tüm referans tipler yeni oluşturuluyor
        public ReportDocument DeepClone()
        {
            var clone = new ReportDocument(
                title: Title,
                content: Content,
                tableData: new List<string>(TableData), // yeni liste
                metadata: Metadata.Clone()  // yeni metadata
                );

            Console.WriteLine($"[ReportDocument] '{Title}' derin kopyalandı.");
            return clone;
        }

        public override string ToString()
    => $"[{DocumentType}] {Title} | {Metadata}";
    }
}