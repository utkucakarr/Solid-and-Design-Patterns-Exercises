using FluentAssertions;
using SRP_Implementation.Models;
using SRP_Implementation.Repositories;

namespace SRP_Tests
{
    public class OrderRepositoryTests
    {
        private readonly OrderRepository _orderRepository = new();

        [Fact]
        public void Save_ShouldAddOrder_ToInMemoryDb()
        {
            var order = CreateSampleOrder(id: 1);

            _orderRepository.Save(order);

            _orderRepository.GetById(1).Should().NotBeNull();
        }

        [Fact]
        public void GetById_ShouldReturnCorrectOrder()
        {
            var order = CreateSampleOrder(id: 42);

            _orderRepository.Save(order);

            var result = _orderRepository.GetById(42);

            result.Should().NotBeNull();
            result!.Id.Should().Be(42);
        }

        [Fact]
        public void GetById_WithNonExistentId_ShouldReturnNull()
        {
            var result = _orderRepository.GetById(999);

            result.Should().BeNull();
        }

        [Fact]
        public void Save_MultiplOrders_ShouldAllBeRetrievable()
        {
            var order1 = CreateSampleOrder(id: 1);
            var order2 = CreateSampleOrder(id: 2);

            _orderRepository.Save(order1);
            _orderRepository.Save(order2);

            _orderRepository.GetById(1).Should().NotBeNull();
            _orderRepository.GetById(2).Should().NotBeNull();
        }

        private static Order CreateSampleOrder(int id) => new()
        {
            Id = id,
            CustomerEmail = "test@example.com",
            Items = new List<OrderItem>
        {
            new() { Name = "Laptop", Price = 25000, Quantity = 1 }
        }
        };
    }
}
