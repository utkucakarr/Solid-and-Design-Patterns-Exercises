using Visitor_Implementation.Interfaces;
using Visitor_Implementation.Models;

namespace Visitor_Implementation.Products
{
    public class SubscriptionProduct : IProduct
    {
        public string Name { get; }
        public decimal BasePrice { get; }
        public int DurationMonths { get; }

        public SubscriptionProduct(string name, decimal basePrice, int durationMonths)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(basePrice, nameof(basePrice));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(durationMonths, nameof(durationMonths));

            Name = name;
            BasePrice = basePrice;
            DurationMonths = durationMonths;
        }

        // Double dispatch: visitor.Visit(this) — this'in tipi SubscriptionProduct
        public VisitResult Accept(IProductVisitor visitor)
        {
            ArgumentNullException.ThrowIfNull(visitor, nameof(visitor));
            return visitor.Visit(this);
        }
    }
}