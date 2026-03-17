using SingleResponsibility.Models;
using SingleResponsibility.Services;

namespace SingleResponsibilityTest.Services
{
    public class PropertyValidatorTests
    {
        private readonly PropertyValidator _validator;

        public PropertyValidatorTests()
        {
            _validator = new PropertyValidator();
        }

        [Fact]
        public void IsValid_WhenDataIsCorrect_ShouldReturnTrue()
        {
            // Arrange (Hazırlık)
            var property = new Property
            {
                Title = "Deniz Manzaralı Villa",
                Price = 5000000,
                Location = "Bodrum"
            };

            // Act (Eylem)
            var result = _validator.IsValid(property);

            // Assert (Doğrulama)
            Assert.True(result);
        }
    }
}
