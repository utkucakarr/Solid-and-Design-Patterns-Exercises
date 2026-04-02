using Prototype_Implementation.Interfaces;
using Prototype_Implementation.Models;

namespace Prototype_Implementation.Documents
{
    public class InvoiceDocument : IDocumentPrototype<InvoiceDocument>
    {
        public string DocumentType => "Invoice";
        public string Title { get; set; }
        public string CustomerName { get; set; }
        public List<string> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public DocumentMetadata Metadata { get; set; }

        public InvoiceDocument(
            string title,
            string customerName,
            List<string> items,
            decimal totalAmount,
            DocumentMetadata metadata)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(customerName, nameof(customerName));
            ArgumentNullException.ThrowIfNull(items, nameof(items));
            ArgumentOutOfRangeException.ThrowIfNegative(totalAmount, nameof(totalAmount));
            ArgumentNullException.ThrowIfNull(metadata, nameof(metadata));

            Title = title;
            CustomerName = customerName;
            Items = items;
            TotalAmount = totalAmount;
            Metadata = metadata;
        }

        // Shallow Copy
        public InvoiceDocument Clone()
        {
            var clone = (InvoiceDocument)MemberwiseClone();
            Console.WriteLine($"[InvoiceDocument] '{Title}' sığ kopyalandı.");
            return clone;
        }

        // Deep Copy
        public InvoiceDocument DeepClone()
        {
            var clone = new InvoiceDocument(
                title: Title,
                customerName: CustomerName,
                items: new List<string>(Items),
                totalAmount: TotalAmount,
                metadata: Metadata.Clone()
                );

            Console.WriteLine($"[InvoiceDocument] '{Title}' derin kopyalandı.");
            return clone;
        }

        public override string ToString()
           => $"[{DocumentType}] {Title} | Müşteri: {CustomerName} | " +
           $"Tutar: {TotalAmount} TL | {Metadata}";
    }
}
