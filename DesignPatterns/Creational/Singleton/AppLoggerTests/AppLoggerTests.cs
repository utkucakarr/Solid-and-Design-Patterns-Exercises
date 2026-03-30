using FluentAssertions;
using Moq;
using Singleton_Implementation.Enums;
using Singleton_Implementation.Interfaces;
using Singleton_Implementation.Logging;
using Singleton_Implementation.Services;

namespace AppLoggerTests
{
    public class AppLoggerTests
    {
        // --- Singleton Garantisi ---
        [Fact]
        public void GetInstance_ShouldReturnSameInstance_WhenCalledMultipleTimes()
        {
            var instance1 = AppLogger.GetInstance();
            var instance2 = AppLogger.GetInstance();
            var instance3 = AppLogger.GetInstance();

            instance1.Should().BeSameAs(instance2);
            instance2.Should().BeSameAs(instance3);
        }

        [Fact]
        public void GetInstance_ShouldReturnSameInstanceId_WhenCalledMuptipleTimes()
        {
            var instance1 = AppLogger.GetInstance();
            var instance2 = AppLogger.GetInstance();

            instance1.InstanceId.Should().Be(instance2.InstanceId);
        }

        [Fact]
        public void GetInstance_ShouldNotReturnNull()
        {
            var instance = AppLogger.GetInstance();
            instance.Should().NotBeNull();
        }

        // Thread-Safe Garantisi
        [Fact]
        public void GetInstance_ShouldReturnSameInstance_WhenCalledFromMultipleThreads()
        {
            var instances = new AppLogger[20];

            var threads = Enumerable.Range(0, 20)
                .Select(i => new Thread(() =>
                    instances[i] = AppLogger.GetInstance()))
                .ToList();

            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());

            instances.Should().AllSatisfy(instance =>
                instance.InstanceId.Should().Be(instances[0].InstanceId));
        }

        [Fact]
        public void Log_WithEmptyMessage_ShouldThrowArgumentException()
        {
            var logger = AppLogger.GetInstance();
            
            var act = () => logger.Log(LogLevel.Info, "");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Log_WithValidMessage_ShouldNotThrow()
        {
            var logger = AppLogger.GetInstance();

            var act = () => logger.Log(LogLevel.Info, "Test mesajý");

            act.Should().NotThrow();
        }

        // --- Servis Testleri - Mock ile ---
        [Fact]
        public void OrderService_CreateOrder_ShouldCallInfoLog()
        {
            var mockLogger = new Mock<IAppLogger>();

            var orderService = new OrderService(mockLogger.Object);
            orderService.CreateOrder("Laptop", 25000);

            mockLogger.Verify(
                l => l.Info(It.Is<string>(m => m.Contains("Laptop") && m.Contains("25000"))),
                Times.Once);
        }

        [Fact]
        public void OrderService_CancelOrder_ShouldCallWarningLog()
        {
            var mockLogger = new Mock<IAppLogger>();

            var orderService = new OrderService(mockLogger.Object);
            orderService.CancelOrder(123);

            mockLogger.Verify(
                l => l.Warning(It.Is<string>(m => m.Contains("123"))),
                Times.Once);
        }

        [Fact]
        public void UserService_RegisterUser_ShouldCallInfoLog()
        {
            var mockLogger = new Mock<IAppLogger>();

            var userService = new UserService(mockLogger.Object);
            userService.RegisterUser("Utku");

            mockLogger.Verify(
                l => l.Info(It.Is<string>(m => m.Contains("Utku"))),
                Times.Once);
        }

        [Fact]
        public void UserService_LoginFailed_ShouldCallWarningLog()
        {
            var mockLogger = new Mock<IAppLogger>();

            var userService = new UserService(mockLogger.Object);
            userService.LoginFailed("hacker");

            mockLogger.Verify(
                l => l.Error(It.Is<string>(m => m.Contains("hacker"))),
                Times.Once);
        }

        [Fact]
        public void PaymentService_ProcessPayment_ShouldCallInfoLog()
        {
            var mockLogger = new Mock<IAppLogger>();

            var paymentService = new PaymentService(mockLogger.Object);
            paymentService.ProcessPayment(25000, "Kredi Kartý");

            mockLogger.Verify(
                l => l.Info(It.Is<string>(m => m.Contains("25000") && m.Contains("Kredi Kartý"))),
                Times.Once);
        }

        [Fact]
        public void PaymentService_PaymentFailed_ShouldCallErrorLog()
        {
            var mockLogger = new Mock<IAppLogger>();

            var paymentService = new PaymentService(mockLogger.Object);
            paymentService.PaymentFailed(25000, "Yetersiz Bakiye");

            mockLogger.Verify(
                l => l.Error(It.Is<string>(m => m.Contains("2500") && m.Contains("Yetersiz Bakiye"))),
                Times.Once);
        }

        // --- Guard Clause Testleri ---
        [Fact]
        public void OrderService_Constructure_WithNullProduct_ShouldThrow()
        {
            var act = () => new OrderService(null);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("logger");
        }

        [Fact]
        public void OrderService_CreateOrder_WithNullProduct_ShouldThrow()
        {
            var mockLogger = new Mock<IAppLogger>();

            var orderService = new OrderService(mockLogger.Object);
            var act = () => orderService.CreateOrder(null, 100);

            act.Should().Throw<ArgumentException>()
                .WithParameterName("product");
        }

        [Fact]
        public void OrderService_CreateOrder_WithNegativePrice_ShouldThrow()
        {
            var mockLogger = new Mock<IAppLogger>();

            OrderService orderService = new OrderService(mockLogger.Object);
            var act = () => orderService.CreateOrder("Laptop", -1);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("price");
        }
    }
}