using Visitor_Implementation.Interfaces;
using Visitor_Implementation.Models;

namespace Visitor_Implementation.Products
{
    public sealed class DigitalProduct : IProduct
    {
        public string Name { get; }
        public decimal BasePrice { get; }
        public string DownloadUrl { get; }

        public DigitalProduct(string name, decimal basePrice, string downloadUrl)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(basePrice, nameof(basePrice));
            ArgumentException.ThrowIfNullOrWhiteSpace(downloadUrl, nameof(downloadUrl));

            Name = name;
            BasePrice = basePrice;
            DownloadUrl = downloadUrl;
        }

        // Double dispatch: visitor.Visit(this) — this'in tipi DigitalProduct
        public VisitResult Accept(IProductVisitor visitor)
        {
            ArgumentNullException.ThrowIfNull(visitor, nameof(visitor));
            return visitor.Visit(this);
        }
    }
}