using FluentAssertions;
using SRP_Implementation.Models;

namespace SRP_Tests
{
    public class OrderTest
    {
        private static Order CreateOrder(params (string name, decimal price, int qty)[] items)
        {
            var order = new Order
            {
                Id = 1,
                CustomerEmail = "test@example.com",
                Items = items.Select(i => new OrderItem
                {
                    Name = i.name,
                    Price = i.price,
                    Quantity = i.qty
                }).ToList()
            };

            return order;
        }

        [Fact]
        public void CalculateTotal_ShouldReturnCorrectSum()
        {
            //Arrange (Hazýrlýk)
            var order = CreateOrder(("Laptop", 25000, 1), ("Mouse", 500, 2));
            //Act (Eylem)
            var total = order.CalculateTotal();
            //Assert (Doðrulama)
            total.Should().Be(26000);
        }

        [Fact]
        public void CalculateTotal_WithEmptyItems_ShouldReturnZero()
        {
            var order = new Order { Id = 1, CustomerEmail = "test@example.com" };

            var total = order.CalculateTotal();

            total.Should().Be(0);
        }

        [Theory]
        [InlineData(100, 3, 300)]
        [InlineData(50, 5, 250)]
        [InlineData(999, 1, 999)]
        public void CalculateTotal_WithSingleItem_ShouldReturnPriceTimesQuantity(
     decimal price, int quantity, decimal expected)
        {
            var order = CreateOrder(("Ürün", price, quantity));

            order.CalculateTotal().Should().Be(expected);
        }

        [Fact]
        public void CalculateTotal_WithMultipleQuantity_ShouldMultiplyCorrectly()
        {
            var order = CreateOrder(("Kalem", 10, 5), ("Defter", 20, 3));

            var total = order.CalculateTotal();

            total.Should().Be(110); // (10*5) + (20*3)
        }
    }
}