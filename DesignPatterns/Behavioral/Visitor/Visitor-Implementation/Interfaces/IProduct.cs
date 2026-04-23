using Visitor_Implementation.Models;

namespace Visitor_Implementation.Interfaces
{
    // Her ürün ziyaretçiyi kabul eder — Accept double dispatch'i tetikler
    public interface IProduct
    {
        string Name { get; }
        decimal BasePrice { get; }
        VisitResult Accept(IProductVisitor visitor);
    }
}
