using Prototype_Implementation.Interfaces;
using Prototype_Implementation.Models;

namespace Prototype_Implementation.Documents
{
    public class ContractDocument : IDocumentPrototype<ContractDocument>
    {
        public string DocumentType => "Contract";
        public string Title { get; set; }
        public List<string> Parties { get; set; }
        public List<string> Clauses { get; set; }
        public DocumentMetadata Metadata { get; set; }

        public ContractDocument(
            string title,
            List<string> parties,
            List<string> clauses,
            DocumentMetadata metadata)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentNullException.ThrowIfNull(parties, nameof(parties));
            ArgumentNullException.ThrowIfNull(clauses, nameof(clauses));
            ArgumentNullException.ThrowIfNull(metadata, nameof(metadata));

            Title = title;
            Parties = parties;
            Clauses = clauses;
            Metadata = metadata;
        }

        // Shallow Copy
        public ContractDocument Clone()
        {
            var clone = (ContractDocument)MemberwiseClone();
            Console.WriteLine($"[ContractDocument] '{Title}' sığ kopyalandı.");
            return clone;
        }

        // Deep Copy
        public ContractDocument DeepClone()
        {
            var clone = new ContractDocument(
                title: Title,
                parties: new List<string>(Parties),
                clauses: new List<string>(Clauses),
                metadata: Metadata.Clone()
                );

            Console.WriteLine($"[ContractDocument] '{Title}' derin kopyalandı.");
            return clone;
        }

        public override string ToString()
            => $"[{DocumentType}] {Title} | Taraflar: {string.Join(", ", Parties)} | {Metadata}";
    }
}