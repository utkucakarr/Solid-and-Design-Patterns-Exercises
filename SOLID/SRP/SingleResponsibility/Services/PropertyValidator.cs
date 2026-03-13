using SingleResponsibility.Interfaces;
using SingleResponsibility.Models;

namespace SingleResponsibility.Services
{
    public class PropertyValidator : IPropertyValidator
    {
        public bool IsValid(Property property)
        {
            // Başlık boş olmalı ve fiyat 0'dan büyük olmalı
            return !string.IsNullOrWhiteSpace(property.Title) && property.Title.Length >= 5 && property.Price > 0;
        }
    }
}
