using SingleResponsibility.Interfaces;
using SingleResponsibility.Models;

namespace SingleResponsibility
{
    public class PropertyManager
    {
        private readonly IPropertyValidator _validator;
        private readonly IPropertyRepository _repository;
        private readonly ILogger _logger;

        // Dependency Injection (Constructor Injection)
        public PropertyManager(IPropertyValidator validator, IPropertyRepository repository, ILogger logger)
        {
            _validator = validator;
            _repository = repository;
            _logger = logger;
        }

        public void AddProperty(Property property)
        {
            if (_validator.IsValid(property))
            {
                _repository.Save(property);
                _logger.LogInfo($"{property.Title} başarıyla eklendi.");
            }
            else
            {
                _logger.LogInfo($"Hata: {property.Title} için veriler geçersiz!");
            }
        }
    }
}
