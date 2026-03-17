using Moq;
using SingleResponsibility;
using SingleResponsibility.Interfaces;
using SingleResponsibility.Models;

namespace SingleResponsibilityTest.Services
{
    public class PropertyManagerTests
    {
        private readonly Mock<IPropertyValidator> _mockValidator;
        private readonly Mock<IPropertyRepository> _mockRepository;
        private readonly Mock<ILogger> _mockLogger;
        private readonly PropertyManager _manager;

        public PropertyManagerTests()
        {
            // Bağımlılıkları taklit ediyoruz (Mocking)
            _mockValidator = new Mock<IPropertyValidator>();
            _mockRepository = new Mock<IPropertyRepository>();
            _mockLogger = new Mock<ILogger>();

            // Taklit nesneleri Manager'a enjekte ediyoruz
            _manager = new PropertyManager(
                _mockValidator.Object,
                _mockRepository.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void AddProperty_WhenValidationSucceeds_ShouldSaveAndLogSuccess()
        {
            // Arrange
            var property = new Property { Title = "Boğazda Yalı", Price = 15000000 };

            // Validatörün 'true' döneceğini garanti ediyoruz
            _mockValidator.Setup(v => v.IsValid(property)).Returns(true);

            // Act
            _manager.AddProperty(property);

            // Assert
            // Repository'deki Save metodu tam olarak 1 kere mi çağrıldı?
            _mockRepository.Verify(r => r.Save(property), Times.Once);

            // Başarı mesajı loglandı mı?
            _mockLogger.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("başarıyla"))), Times.Once);
        }

        [Fact]
        public void AddProperty_WhenValidationFails_ShouldNotSaveAndLogWarning()
        {
            // Arrange
            var property = new Property { Title = "Geçersiz", Price = -50 };

            // Validatörün 'false' döneceğini simüle ediyoruz
            _mockValidator.Setup(v => v.IsValid(property)).Returns(false);

            // Act
            _manager.AddProperty(property);

            // Assert
            // Validasyon başarısız olduğu için 'Save' metodu ASLA (Never) çağrılmamalı!
            _mockRepository.Verify(r => r.Save(It.IsAny<Property>()), Times.Never);

            // Hata mesajı loglandı mı?
            _mockLogger.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Hata"))), Times.Once);
        }
    }
}
