using Visitor_Implementation.Models;
using Visitor_Implementation.Products;

namespace Visitor_Implementation.Interfaces
{
    // Her ürün tipi için ayrı Visit overload'u — double dispatch'in kalbi
    public interface IProductVisitor
    {
        VisitResult Visit(PhysicalProduct product);
        VisitResult Visit(DigitalProduct product);
        VisitResult Visit(SubscriptionProduct product);
    }
}
