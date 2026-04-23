using Visitor_Implementation.Interfaces;
using Visitor_Implementation.Models;

namespace Visitor_Implementation.Products
{
    public sealed class PhysicalProduct : IProduct
    {
        public string Name { get; }
        public decimal BasePrice { get; }
        public decimal WeightKg { get; }

        public PhysicalProduct(string name, decimal basePrice, decimal weightKg)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(basePrice, nameof(basePrice));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(weightKg, nameof(weightKg));

            Name = name;
            BasePrice = basePrice;
            WeightKg = weightKg;
        }

        // Double dispatch: visitor.Visit(this) — this'in tipi PhysicalProduct
        public VisitResult Accept(IProductVisitor visitor)
        {
            ArgumentNullException.ThrowIfNull(visitor, nameof(visitor));
            return visitor.Visit(this);
        }
    }
}
