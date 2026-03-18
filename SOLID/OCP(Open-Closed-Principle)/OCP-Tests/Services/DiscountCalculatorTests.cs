using OCP_Implementation.Services;
using OCP_Implementation.Strategies;

namespace OCP_Tests.Services
{
    public class DiscountCalculatorTests
    {
        private readonly DiscountCalculator _calculator;

        public DiscountCalculatorTests()
        {
            _calculator = new DiscountCalculator();
        }

        [Theory]
        [InlineData(100, 5)]   // 1. senaryo: 100 TL verince 5 TL indirim bekliyorum.
        [InlineData(1000, 50)] // 2. senaryo: 1000 TL verince 50 TL indirim bekliyorum.
        public void Calculate_ShouldReturnCorrectStandardDiscount(decimal amount, decimal expected)
        {
            // Arrange
            var strategy = new StandartDiscount();

            // Act
            var result = _calculator.Calculate(amount, strategy);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Calculate_WithPremium_ShouldReturnTenPercent()
        {
            // Act
            var strategyPremium = new PremiumDiscount();

            // Arrange & Act
            var result = _calculator.Calculate(200, strategyPremium);

            // Assert
            Assert.Equal(20, result);
        }
    }
}