using SingleResponsibility.Interfaces;
using SingleResponsibility.Models;

namespace SingleResponsibility.Services
{
    public class PropertyRepository : IPropertyRepository
    {
        private const string FilePath = "Properties.txt";

        public void Save(Property property)
        {
            // Basit bir şekilde dosyaya kaydedelim
            var record = $"{property.Id},{property.Title},{property.Price},{property.Location}";
            File.AppendAllLines(FilePath, new[] { record });
        }
    }
}
