using SingleResponsibility.Models;

namespace SingleResponsibility.Interfaces
{
    public interface IPropertyRepository
    {
        void Save(Property property);
    }
}
