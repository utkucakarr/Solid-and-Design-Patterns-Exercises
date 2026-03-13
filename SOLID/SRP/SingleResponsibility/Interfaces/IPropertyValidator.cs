using SingleResponsibility.Models;

namespace SingleResponsibility.Interfaces
{
    public interface IPropertyValidator
    {
        bool IsValid(Property property);
    }
}
