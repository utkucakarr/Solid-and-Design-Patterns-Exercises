using FluentAssertions;
using Moq;
using SRP_Implementation.Interfeces;
using SRP_Implementation.Models;
using SRP_Implementation.Services;

namespace SRP_Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockRepo;
        private readonly Mock<INotificationService> _mockNotification;
        private readonly Mock<IOrderLogger> _mockLogger;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockRepo = new Mock<IOrderRepository>();
            _mockNotification = new Mock<INotificationService>();
            _mockLogger = new Mock<IOrderLogger>();

            _orderService = new OrderService(
                _mockRepo.Object,
                _mockNotification.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public void ProcessOrder_ShouldCallSave_Once()
        {
            var order = CreateSampleOrder();

            _orderService.ProcessOrder(order);

            _mockRepo.Verify(r => r.Save(order), Times.Once);
        }

        [Fact]
        public void ProcessOrder_ShouldSendConfirmation_Once()
        {
            var order = CreateSampleOrder();

            _orderService.ProcessOrder(order);

            _mockNotification.Verify(n => n.SendConfirmation(order), Times.Once);
        }

        [Fact]
        public void ProcessOrder_ShouldLogOrderCreated_Once()
        {
            var order = CreateSampleOrder();

            _orderService.ProcessOrder(order);

            _mockLogger.Verify(l => l.LogOrderCreated(order), Times.Once);
        }

        [Fact]
        public void ProcessOrder_ShouldCallAllDependencies_InOrder()
        {
            var order = CreateSampleOrder();
            var callOrder = new List<string>();

            _mockRepo.Setup(r => r.Save(It.IsAny<Order>()))
                     .Callback(() => callOrder.Add("save"));

            _mockNotification.Setup(n => n.SendConfirmation(It.IsAny<Order>()))
                             .Callback(() => callOrder.Add("notify"));

            _mockLogger.Setup(l => l.LogOrderCreated(It.IsAny<Order>()))
                       .Callback(() => callOrder.Add("log"));

            _orderService.ProcessOrder(order);

            callOrder.Should().ContainInOrder("save", "notify", "log");
        }

        [Fact]
        public void ProcessOrder_ShouldNotCallNotification_WhenRepositoryThrows()
        {
            var order = CreateSampleOrder();

            _mockRepo
                .Setup(r => r.Save(It.IsAny<Order>()))
                .Throws(new Exception("DB hatası."));

            var act = () => _orderService.ProcessOrder(order);

            act.Should().Throw<Exception>();

            _mockNotification.Verify(
                n => n.SendConfirmation(It.IsAny<Order>()),
                Times.Never
            );
        }

        [Fact]
        public void ProcessOrder_ShouldNotCallLogger_WhenRepositoryThrows()
        {
            var order = CreateSampleOrder();

            _mockRepo
                .Setup(r => r.Save(It.IsAny<Order>()))
                .Throws(new Exception("DB hatası."));

            var act = () => _orderService.ProcessOrder(order);

            act.Should().Throw<Exception>();

            _mockLogger.Verify(
                l => l.LogOrderCreated(It.IsAny<Order>()),
                Times.Never
            );
        }

        // ─── Yardımcı Metot ────────────────────────────────────

        private static Order CreateSampleOrder() => new()
        {
            Id = 1,
            CustomerEmail = "test@example.com",
            Items = new List<OrderItem>
        {
            new() { Name = "Laptop", Price = 25000, Quantity = 1 },
            new() { Name = "Mouse",  Price = 500,   Quantity = 2 }
        }
        };
    }
}
