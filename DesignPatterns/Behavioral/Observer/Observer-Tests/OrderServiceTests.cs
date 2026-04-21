using FluentAssertions;
using Moq;
using Observer_Implementation.Interfaces;
using Observer_Implementation.Models;
using Observer_Implementation.Observers;
using Observer_Implementation.Subject;

namespace Observer_Tests
{
    public class OrderServiceTests
    {
        #region Helpers

        private static Order CreateOrder(
            string orderId = "ORD-001",
            string email = "test@example.com",
            string phone = "+905551234567",
            string deviceToken = "device-token-abc",
            string productName = "MacBook Pro",
            int quantity = 1,
            decimal totalPrice = 85000m) =>
            new(orderId, email, phone, deviceToken, productName, quantity, totalPrice);

        #endregion

        // HAPPY PATH — Başarılı senaryolar

        [Fact]
        public void PlaceOrder_WithValidOrder_ShouldReturnSuccess()
        {
            // Arrange
            var service = new OrderService();
            var order = CreateOrder();

            // Act
            var result = service.PlaceOrder(order);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.OrderId.Should().Be("ORD-001");
            result.NewStatus.Should().Be(OrderStatus.Placed);
        }

        [Fact]
        public void ChangeOrderStatus_WithValidTransition_ShouldReturnSuccess()
        {
            // Arrange
            var service = new OrderService();
            var order = CreateOrder();
            service.PlaceOrder(order);

            // Act
            var result = service.ChangeOrderStatus("ORD-001", OrderStatus.Confirmed);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.OrderId.Should().Be("ORD-001");
            result.NewStatus.Should().Be(OrderStatus.Confirmed);
        }

        [Fact]
        public void ChangeOrderStatus_WithObservers_ShouldReturnCorrectObserverCount()
        {
            // Arrange
            var service = new OrderService();
            var mockObserver1 = new Mock<IOrderObserver>();
            var mockObserver2 = new Mock<IOrderObserver>();

            service.Subscribe(mockObserver1.Object);
            service.Subscribe(mockObserver2.Object);

            var order = CreateOrder();
            service.PlaceOrder(order);

            // Act
            var result = service.ChangeOrderStatus("ORD-001", OrderStatus.Confirmed);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.NotifiedObserverCount.Should().Be(2);
        }

        // OBSERVER ÇAĞRI DOĞRULAMA — Verify testleri

        [Fact]
        public void PlaceOrder_ShouldNotifyAllSubscribedObservers()
        {
            // Arrange
            var service = new OrderService();
            var mockObserver1 = new Mock<IOrderObserver>();
            var mockObserver2 = new Mock<IOrderObserver>();

            service.Subscribe(mockObserver1.Object);
            service.Subscribe(mockObserver2.Object);

            var order = CreateOrder();

            // Act
            service.PlaceOrder(order);

            // Assert
            mockObserver1.Verify(
                x => x.OnOrderStatusChanged(order, OrderStatus.Placed),
                Times.Once);

            mockObserver2.Verify(
                x => x.OnOrderStatusChanged(order, OrderStatus.Placed),
                Times.Once);
        }

        [Fact]
        public void Unsubscribe_ShouldNotNotifyRemovedObserver()
        {
            // Arrange
            var service = new OrderService();
            var mockObserver = new Mock<IOrderObserver>();
            service.Subscribe(mockObserver.Object);
            service.Unsubscribe(mockObserver.Object);

            var order = CreateOrder();
            service.PlaceOrder(order);

            // Act
            service.ChangeOrderStatus("ORD-001", OrderStatus.Confirmed);

            // Assert
            mockObserver.Verify(
                x => x.OnOrderStatusChanged(It.IsAny<Order>(), It.IsAny<OrderStatus>()),
                Times.Never);
        }

        [Fact]
        public void Subscribe_SamObserverTwice_ShouldNotifyOnlyOnce()
        {
            // Arrange
            var service = new OrderService();
            var mockObserver = new Mock<IOrderObserver>();

            service.Subscribe(mockObserver.Object);
            service.Subscribe(mockObserver.Object); // aynı observer iki kez

            var order = CreateOrder();
            service.PlaceOrder(order);

            // Act
            service.ChangeOrderStatus("ORD-001", OrderStatus.Confirmed);

            // Assert
            mockObserver.Verify(
                x => x.OnOrderStatusChanged(It.IsAny<Order>(), It.IsAny<OrderStatus>()),
                Times.Exactly(2)); // PlaceOrder + ChangeOrderStatus
        }

        // EMAIL OBSERVER — Bildirim doğrulama

        [Fact]
        public void EmailObserver_WhenOrderConfirmed_ShouldSendEmail()
        {
            // Arrange
            var mockEmailNotifier = new Mock<IEmailNotifier>();
            var observer = new EmailNotificationObserver(mockEmailNotifier.Object);
            var order = CreateOrder();
            order.UpdateStatus(OrderStatus.Confirmed);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Placed);

            // Assert
            mockEmailNotifier.Verify(
                x => x.Send(
                    "test@example.com",
                    It.Is<string>(s => s.Contains("ORD-001") && s.Contains("Onaylandı")),
                    It.Is<string>(s => s.Contains("MacBook Pro"))),
                Times.Once);
        }

        [Fact]
        public void EmailObserver_WhenOrderShipped_ShouldSendEmail()
        {
            // Arrange
            var mockEmailNotifier = new Mock<IEmailNotifier>();
            var observer = new EmailNotificationObserver(mockEmailNotifier.Object);
            var order = CreateOrder();
            order.UpdateStatus(OrderStatus.Shipped);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Confirmed);

            // Assert
            mockEmailNotifier.Verify(
                x => x.Send(
                    "test@example.com",
                    It.Is<string>(s => s.Contains("Kargoya")),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public void EmailObserver_WhenOrderDelivered_ShouldSendEmail()
        {
            // Arrange
            var mockEmailNotifier = new Mock<IEmailNotifier>();
            var observer = new EmailNotificationObserver(mockEmailNotifier.Object);
            var order = CreateOrder();
            order.UpdateStatus(OrderStatus.Delivered);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Shipped);

            // Assert
            mockEmailNotifier.Verify(
                x => x.Send(
                    "test@example.com",
                    It.Is<string>(s => s.Contains("Teslim")),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public void EmailObserver_WhenOrderCancelled_ShouldSendEmail()
        {
            // Arrange
            var mockEmailNotifier = new Mock<IEmailNotifier>();
            var observer = new EmailNotificationObserver(mockEmailNotifier.Object);
            var order = CreateOrder();
            order.UpdateStatus(OrderStatus.Cancelled);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Confirmed);

            // Assert
            mockEmailNotifier.Verify(
                x => x.Send(
                    "test@example.com",
                    It.Is<string>(s => s.Contains("İptal")),
                    It.IsAny<string>()),
                Times.Once);
        }

        // SMS OBSERVER — Kritik durumlar

        [Fact]
        public void SmsObserver_WhenOrderConfirmed_ShouldSendSms()
        {
            // Arrange
            var mockSmsNotifier = new Mock<ISmsNotifier>();
            var observer = new SmsNotificationObserver(mockSmsNotifier.Object);
            var order = CreateOrder();
            order.UpdateStatus(OrderStatus.Confirmed);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Placed);

            // Assert
            mockSmsNotifier.Verify(
                x => x.Send(
                    "+905551234567",
                    It.Is<string>(s => s.Contains("onaylandı"))),
                Times.Once);
        }

        [Fact]
        public void SmsObserver_WhenStatusIsPlaced_ShouldNotSendSms()
        {
            // Arrange
            var mockSmsNotifier = new Mock<ISmsNotifier>();
            var observer = new SmsNotificationObserver(mockSmsNotifier.Object);
            var order = CreateOrder(); // Status = Placed

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Placed);

            // Assert — Placed durumunda SMS gönderilmemeli
            mockSmsNotifier.Verify(
                x => x.Send(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        // PUSH OBSERVER — Yalnızca Shipped ve Delivered

        [Fact]
        public void PushObserver_WhenOrderShipped_ShouldSendPush()
        {
            // Arrange
            var mockPushNotifier = new Mock<IPushNotifier>();
            var observer = new PushNotificationObserver(mockPushNotifier.Object);
            var order = CreateOrder();
            order.UpdateStatus(OrderStatus.Shipped);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Confirmed);

            // Assert
            mockPushNotifier.Verify(
                x => x.Send(
                    "device-token-abc",
                    It.Is<string>(s => s.Contains("Yola Çıktı")),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public void PushObserver_WhenOrderConfirmed_ShouldNotSendPush()
        {
            // Arrange
            var mockPushNotifier = new Mock<IPushNotifier>();
            var observer = new PushNotificationObserver(mockPushNotifier.Object);
            var order = CreateOrder();
            order.UpdateStatus(OrderStatus.Confirmed);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Placed);

            // Assert — Confirmed durumunda Push gönderilmemeli
            mockPushNotifier.Verify(
                x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        // INVENTORY OBSERVER — Stok rezerve ve iade

        [Fact]
        public void InventoryObserver_WhenOrderConfirmed_ShouldReserveStock()
        {
            // Arrange
            var mockInventory = new Mock<IInventoryService>();
            var observer = new InventoryObserver(mockInventory.Object);
            var order = CreateOrder(quantity: 3);
            order.UpdateStatus(OrderStatus.Confirmed);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Placed);

            // Assert
            mockInventory.Verify(
                x => x.ReserveStock("MacBook Pro", 3),
                Times.Once);

            mockInventory.Verify(
                x => x.ReleaseStock(It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public void InventoryObserver_WhenOrderCancelled_ShouldReleaseStock()
        {
            // Arrange
            var mockInventory = new Mock<IInventoryService>();
            var observer = new InventoryObserver(mockInventory.Object);
            var order = CreateOrder(quantity: 2);
            order.UpdateStatus(OrderStatus.Cancelled);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Confirmed);

            // Assert
            mockInventory.Verify(
                x => x.ReleaseStock("MacBook Pro", 2),
                Times.Once);

            mockInventory.Verify(
                x => x.ReserveStock(It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public void InventoryObserver_WhenOrderShipped_ShouldNotTouchStock()
        {
            // Arrange
            var mockInventory = new Mock<IInventoryService>();
            var observer = new InventoryObserver(mockInventory.Object);
            var order = CreateOrder();
            order.UpdateStatus(OrderStatus.Shipped);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Confirmed);

            // Assert — Shipped durumunda stok işlemi olmamalı
            mockInventory.Verify(
                x => x.ReserveStock(It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);

            mockInventory.Verify(
                x => x.ReleaseStock(It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);
        }

        // INVOICE OBSERVER — Yalnızca Delivered

        [Fact]
        public void InvoiceObserver_WhenOrderDelivered_ShouldCreateInvoice()
        {
            // Arrange
            var mockInvoice = new Mock<IInvoiceService>();
            var observer = new InvoiceObserver(mockInvoice.Object);
            var order = CreateOrder(totalPrice: 85000m);
            order.UpdateStatus(OrderStatus.Delivered);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Shipped);

            // Assert
            mockInvoice.Verify(
                x => x.CreateInvoice("ORD-001", 85000m),
                Times.Once);
        }

        [Fact]
        public void InvoiceObserver_WhenOrderNotDelivered_ShouldNotCreateInvoice()
        {
            // Arrange
            var mockInvoice = new Mock<IInvoiceService>();
            var observer = new InvoiceObserver(mockInvoice.Object);
            var order = CreateOrder();
            order.UpdateStatus(OrderStatus.Shipped);

            // Act
            observer.OnOrderStatusChanged(order, OrderStatus.Confirmed);

            // Assert — Shipped durumunda fatura oluşturulmamalı
            mockInvoice.Verify(
                x => x.CreateInvoice(It.IsAny<string>(), It.IsAny<decimal>()),
                Times.Never);
        }

        // BAŞARISIZ SENARYOLAR

        [Fact]
        public void ChangeOrderStatus_WhenOrderNotFound_ShouldReturnFail()
        {
            // Arrange
            var service = new OrderService();

            // Act
            var result = service.ChangeOrderStatus("NON-EXISTENT", OrderStatus.Confirmed);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("bulunamadı");
        }

        [Fact]
        public void ChangeOrderStatus_WhenSameStatus_ShouldReturnFail()
        {
            // Arrange
            var service = new OrderService();
            var order = CreateOrder();
            service.PlaceOrder(order);

            // Act
            var result = service.ChangeOrderStatus("ORD-001", OrderStatus.Placed);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("zaten");
        }

        [Fact]
        public void ChangeOrderStatus_WhenOrderNotFound_ShouldNotNotifyObservers()
        {
            // Arrange
            var service = new OrderService();
            var mockObserver = new Mock<IOrderObserver>();
            service.Subscribe(mockObserver.Object);

            // Act
            service.ChangeOrderStatus("NON-EXISTENT", OrderStatus.Confirmed);

            // Assert
            mockObserver.Verify(
                x => x.OnOrderStatusChanged(It.IsAny<Order>(), It.IsAny<OrderStatus>()),
                Times.Never);
        }

        // GUARD CLAUSE TESTLERİ

        [Fact]
        public void PlaceOrder_WhenOrderIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var service = new OrderService();

            // Act
            var act = () => service.PlaceOrder(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("order");
        }

        [Fact]
        public void ChangeOrderStatus_WhenOrderIdIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var service = new OrderService();

            // Act
            var act = () => service.ChangeOrderStatus(string.Empty, OrderStatus.Confirmed);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("orderId");
        }

        [Fact]
        public void ChangeOrderStatus_WhenOrderIdIsWhitespace_ShouldThrowArgumentException()
        {
            // Arrange
            var service = new OrderService();

            // Act
            var act = () => service.ChangeOrderStatus("   ", OrderStatus.Confirmed);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("orderId");
        }

        [Fact]
        public void Subscribe_WhenObserverIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var service = new OrderService();

            // Act
            var act = () => service.Subscribe(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("observer");
        }

        [Fact]
        public void Unsubscribe_WhenObserverIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var service = new OrderService();

            // Act
            var act = () => service.Unsubscribe(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("observer");
        }

        // CONSTRUCTOR NULL GUARD TESTLERİ

        [Fact]
        public void EmailObserver_WhenEmailNotifierIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            var act = () => new EmailNotificationObserver(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("emailNotifier");
        }

        [Fact]
        public void SmsObserver_WhenSmsNotifierIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            var act = () => new SmsNotificationObserver(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("smsNotifier");
        }

        [Fact]
        public void PushObserver_WhenPushNotifierIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            var act = () => new PushNotificationObserver(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("pushNotifier");
        }

        [Fact]
        public void InventoryObserver_WhenInventoryServiceIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            var act = () => new InventoryObserver(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("inventoryService");
        }

        [Fact]
        public void InvoiceObserver_WhenInvoiceServiceIsNull_ShouldThrowArgumentNullException()
        {
            // Act
            var act = () => new InvoiceObserver(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("invoiceService");
        }

        [Fact]
        public void Order_WhenOrderIdIsEmpty_ShouldThrowArgumentException()
        {
            // Act
            var act = () => CreateOrder(orderId: string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("orderId");
        }

        [Fact]
        public void Order_WhenQuantityIsZero_ShouldThrowArgumentOutOfRangeException()
        {
            // Act
            var act = () => CreateOrder(quantity: 0);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("quantity");
        }

        [Fact]
        public void Order_WhenTotalPriceIsNegative_ShouldThrowArgumentOutOfRangeException()
        {
            // Act
            var act = () => CreateOrder(totalPrice: -1m);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("totalPrice");
        }
    }
}