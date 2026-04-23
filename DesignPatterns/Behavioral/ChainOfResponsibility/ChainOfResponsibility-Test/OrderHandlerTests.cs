using ChainOfResponsibility_Implementation.Handlers;
using ChainOfResponsibility_Implementation.Models;
using FluentAssertions;
using Xunit;

namespace ChainOfResponsibility_Test
{
    public class OrderHandlerTests
    {
        // Zinciri her test için temiz kur
        private (StockHandler stock, FraudHandler fraud, PaymentHandler payment, ShippingHandler shipping)
            BuildChain()
        {
            var stock = new StockHandler();
            var fraud = new FraudHandler();
            var payment = new PaymentHandler();
            var shipping = new ShippingHandler();

            stock.SetNext(fraud).SetNext(payment).SetNext(shipping);

            return (stock, fraud, payment, shipping);
        }

        // 1. HAPPY PATH — Başarılı senaryo

        [Fact]
        public void Handle_WhenAllConditionsMet_ShouldReturnSuccess()
        {
            // Arrange
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("PROD-001", "CUST-001", 1, 5000m);

            // Act
            var result = stock.Handle(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.TrackingCode.Should().NotBeNullOrWhiteSpace();
            result.FailedHandler.Should().BeNull();
            result.Message.Should().Be("Sipariş başarıyla onaylandı.");
        }

        [Fact]
        public void Handle_WhenAllConditionsMet_ShouldGenerateUniqueTrackingCodes()
        {
            // Arrange
            var (stock1, _, _, _) = BuildChain();
            var (stock2, _, _, _) = BuildChain();
            var request = new OrderRequest("PROD-001", "CUST-001", 1, 5000m);

            // Act
            var result1 = stock1.Handle(request);
            var result2 = stock2.Handle(request);

            // Assert
            result1.TrackingCode.Should().NotBe(result2.TrackingCode);
        }

        // 2. STOK HANDLER TESTLERİ

        [Fact]
        public void Handle_WhenStockInsufficient_ShouldFailAtStockHandler()
        {
            // Arrange
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("PROD-002", "CUST-001", 5, 5000m);

            // Act
            var result = stock.Handle(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.FailedHandler.Should().Be(nameof(StockHandler));
            result.TrackingCode.Should().BeNull();
            result.Message.Should().Contain("Stok yetersiz");
        }

        [Fact]
        public void Handle_WhenProductNotFound_ShouldFailAtStockHandler()
        {
            // Arrange
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("UNKNOWN-PROD", "CUST-001", 1, 5000m);

            // Act
            var result = stock.Handle(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.FailedHandler.Should().Be(nameof(StockHandler));
            result.Message.Should().Contain("Stok yetersiz");
        }

        [Fact]
        public void Handle_WhenQuantityExceedsStock_ShouldFailAtStockHandler()
        {
            // Arrange — PROD-003 stokta 10 adet var
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("PROD-003", "CUST-001", 11, 5000m);

            // Act
            var result = stock.Handle(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.FailedHandler.Should().Be(nameof(StockHandler));
        }

        // 3. FRAUD HANDLER TESTLERİ

        [Fact]
        public void Handle_WhenFraudDetected_ShouldFailAtFraudHandler()
        {
            // Arrange
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("PROD-001", "CUST-FRAUD", 1, 5000m);

            // Act
            var result = stock.Handle(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.FailedHandler.Should().Be(nameof(FraudHandler));
            result.TrackingCode.Should().BeNull();
            result.Message.Should().Contain("Şüpheli işlem");
        }

        [Fact]
        public void Handle_WhenFraudDetected_ShouldNotReachPaymentHandler()
        {
            // Arrange — stok geçer ama fraud takılır
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("PROD-001", "CUST-FRAUD", 1, 5000m);

            // Act
            var result = stock.Handle(request);

            // Assert — ödeme aşamasına hiç gelinmemeli
            result.IsSuccess.Should().BeFalse();
            result.FailedHandler.Should().Be(nameof(FraudHandler));
            result.Message.Should().NotContain("Bakiye");
        }

        [Fact]
        public void Handle_WhenFraudScoreNormal_ShouldPassFraudHandler()
        {
            // Arrange — CUST-002 fraud skoru 30 (eşiğin altında)
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("PROD-001", "CUST-002", 1, 5000m);

            // Act
            var result = stock.Handle(request);

            // Assert — fraud'dan geçmeli
            result.FailedHandler.Should().NotBe(nameof(FraudHandler));
        }

        // 4. PAYMENT HANDLER TESTLERİ

        [Fact]
        public void Handle_WhenBalanceInsufficient_ShouldFailAtPaymentHandler()
        {
            // Arrange — 5 adet * 299.99 = 1499.95, bakiye 100
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("PROD-001", "CUST-001", 5, 100m);

            // Act
            var result = stock.Handle(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.FailedHandler.Should().Be(nameof(PaymentHandler));
            result.Message.Should().Contain("Bakiye yetersiz");
        }

        [Fact]
        public void Handle_WhenBalanceExact_ShouldPassPaymentHandler()
        {
            // Arrange — 1 adet * 299.99 = tam 299.99
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("PROD-001", "CUST-001", 1, 299.99m);

            // Act
            var result = stock.Handle(request);

            // Assert — payment'tan geçmeli
            result.FailedHandler.Should().NotBe(nameof(PaymentHandler));
        }

        // 5. SHIPPING HANDLER TESTLERİ

        [Fact]
        public void Handle_WhenDigitalProduct_ShouldFailAtShippingHandler()
        {
            // Arrange
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("DIGITAL-ONLY", "CUST-001", 1, 5000m);

            // Act
            var result = stock.Handle(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.FailedHandler.Should().Be(nameof(ShippingHandler));
            result.Message.Should().Contain("kargo hizmeti mevcut değil");
        }

        // 6. ZİNCİR SIRASI TESTLERİ

        [Fact]
        public void Handle_WhenStockFails_ShouldNotCheckFraud()
        {
            // Arrange — stokta sıfır ürün var, fraud kullanıcısıyla dene
            var (stock, _, _, _) = BuildChain();
            var request = new OrderRequest("PROD-002", "CUST-FRAUD", 1, 5000m);

            // Act
            var result = stock.Handle(request);

            // Assert — stokta takılmalı, fraud'a hiç gelinmemeli
            result.FailedHandler.Should().Be(nameof(StockHandler));
            result.Message.Should().NotContain("Şüpheli");
        }

        [Fact]
        public void Handle_WhenChainHasNoNext_ShouldReturnSuccess()
        {
            // Arrange — tek başına sadece ShippingHandler
            var shipping = new ShippingHandler();
            var request = new OrderRequest("PROD-001", "CUST-001", 1, 5000m);

            // Act
            var result = shipping.Handle(request);

            // Assert — sonraki yok, başarı dönmeli
            result.IsSuccess.Should().BeTrue();
            result.TrackingCode.Should().NotBeNullOrWhiteSpace();
        }

        // 7. GUARD CLAUSE TESTLERİ

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void OrderRequest_WhenProductIdNullOrWhiteSpace_ShouldThrowArgumentException(string? productId)
        {
            // Act
            var act = () => new OrderRequest(productId!, "CUST-001", 1, 5000m);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithParameterName("productId");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void OrderRequest_WhenCustomerIdNullOrWhiteSpace_ShouldThrowArgumentException(string? customerId)
        {
            // Act
            var act = () => new OrderRequest("PROD-001", customerId!, 1, 5000m);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithParameterName("customerId");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void OrderRequest_WhenQuantityNegativeOrZero_ShouldThrowArgumentOutOfRangeException(int quantity)
        {
            // Act
            var act = () => new OrderRequest("PROD-001", "CUST-001", quantity, 5000m);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithParameterName("quantity");
        }

        [Fact]
        public void OrderRequest_WhenBalanceNegative_ShouldThrowArgumentOutOfRangeException()
        {
            // Act
            var act = () => new OrderRequest("PROD-001", "CUST-001", 1, -1m);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithParameterName("accountBalance");
        }

        // 8. CONSTRUCTOR NULL GUARD TESTLERİ

        [Fact]
        public void StockHandler_Handle_WhenRequestIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var handler = new StockHandler();

            // Act
            var act = () => handler.Handle(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("request");
        }

        [Fact]
        public void FraudHandler_Handle_WhenRequestIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var handler = new FraudHandler();

            // Act
            var act = () => handler.Handle(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("request");
        }

        [Fact]
        public void PaymentHandler_Handle_WhenRequestIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var handler = new PaymentHandler();

            // Act
            var act = () => handler.Handle(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("request");
        }

        [Fact]
        public void ShippingHandler_Handle_WhenRequestIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var handler = new ShippingHandler();

            // Act
            var act = () => handler.Handle(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("request");
        }

        [Fact]
        public void BaseOrderHandler_SetNext_WhenNextIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var handler = new StockHandler();

            // Act
            var act = () => handler.SetNext(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("next");
        }

        // 9. RESULT NESNESİ TESTLERİ

        [Fact]
        public void OrderResult_Success_ShouldHaveCorrectProperties()
        {
            // Act
            var result = OrderResult.Success("TRK-12345678");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.TrackingCode.Should().Be("TRK-12345678");
            result.FailedHandler.Should().BeNull();
            result.Message.Should().Be("Sipariş başarıyla onaylandı.");
        }

        [Fact]
        public void OrderResult_Fail_ShouldHaveCorrectProperties()
        {
            // Act
            var result = OrderResult.Fail("Stok yetersiz", "StockHandler");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Stok yetersiz");
            result.FailedHandler.Should().Be("StockHandler");
            result.TrackingCode.Should().BeNull();
        }
    }
}