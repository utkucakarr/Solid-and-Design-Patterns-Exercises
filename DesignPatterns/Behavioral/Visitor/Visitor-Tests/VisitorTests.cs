using FluentAssertions;
using Visitor_Implementation.Models;
using Visitor_Implementation.Products;
using Visitor_Implementation.Visitors;

namespace Visitor_Tests
{
    public class VisitorTests
    {
        // 1. HAPPY PATH — TaxCalculatorVisitor

        [Fact]
        public void TaxVisitor_WhenPhysicalProduct_ShouldCalculateKdvPlusWeightTax()
        {
            // Arrange — 1000₺ * 0.18 + 2.5kg * 0.5 = 180 + 1.25 = 181.25
            var product = new PhysicalProduct("Laptop", 1000m, 2.5m);
            var visitor = new TaxCalculatorVisitor();

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(181.25m);
            result.Message.Should().Contain("Laptop");
            result.Message.Should().Contain("KDV");
        }

        [Fact]
        public void TaxVisitor_WhenDigitalProduct_ShouldApplyReducedKdvRate()
        {
            // Arrange — 500₺ * 0.08 = 40
            var product = new DigitalProduct("Adobe CC", 500m, "https://adobe.com/download");
            var visitor = new TaxCalculatorVisitor();

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(40m);
            result.Message.Should().Contain("Dijital");
        }

        [Fact]
        public void TaxVisitor_WhenSubscriptionProduct_ShouldMultiplyByDurationMonths()
        {
            // Arrange — 199₺ * 0.18 * 12 ay = 429.84
            var product = new SubscriptionProduct("Netflix", 199m, 12);
            var visitor = new TaxCalculatorVisitor();

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(429.84m);
            result.Message.Should().Contain("12 ay");
        }

        [Fact]
        public void TaxVisitor_WhenPhysicalProductHeavy_ShouldIncreaseWeightTax()
        {
            // Arrange — ağır ürün: 10kg * 0.5 = 5₺ ek vergi
            var lightProduct = new PhysicalProduct("Kalem", 100m, 0.1m);
            var heavyProduct = new PhysicalProduct("Buzdolabı", 100m, 50m);
            var visitor = new TaxCalculatorVisitor();

            // Act
            var lightResult = lightProduct.Accept(visitor);
            var heavyResult = heavyProduct.Accept(visitor);

            // Assert — ağır ürün daha fazla vergi ödemeli
            heavyResult.Amount.Should().BeGreaterThan(lightResult.Amount!.Value);
        }

        // 2. HAPPY PATH — DiscountVisitor

        [Fact]
        public void DiscountVisitor_WhenPremiumAndPhysical_ShouldApply15PercentDiscount()
        {
            // Arrange — 1000₺ * 0.15 = 150
            var product = new PhysicalProduct("Monitör", 1000m, 5m);
            var visitor = new DiscountVisitor(isPremiumCustomer: true);

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(150m);
            result.Message.Should().Contain("15");
        }

        [Fact]
        public void DiscountVisitor_WhenStandardAndPhysical_ShouldApply5PercentDiscount()
        {
            // Arrange — 1000₺ * 0.05 = 50
            var product = new PhysicalProduct("Monitör", 1000m, 5m);
            var visitor = new DiscountVisitor(isPremiumCustomer: false);

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(50m);
            result.Message.Should().Contain("5");
        }

        [Fact]
        public void DiscountVisitor_WhenPremiumAndDigital_ShouldApply20PercentDiscount()
        {
            // Arrange — 500₺ * 0.20 = 100
            var product = new DigitalProduct("JetBrains", 500m, "https://jetbrains.com");
            var visitor = new DiscountVisitor(isPremiumCustomer: true);

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(100m);
            result.Message.Should().Contain("20");
        }

        [Fact]
        public void DiscountVisitor_WhenStandardAndDigital_ShouldApply10PercentDiscount()
        {
            // Arrange — 500₺ * 0.10 = 50
            var product = new DigitalProduct("JetBrains", 500m, "https://jetbrains.com");
            var visitor = new DiscountVisitor(isPremiumCustomer: false);

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(50m);
        }

        [Fact]
        public void DiscountVisitor_WhenPremiumAndSubscription_ShouldApply25PercentDiscount()
        {
            // Arrange — 199₺ * 0.25 = 49.75
            var product = new SubscriptionProduct("Netflix", 199m, 12);
            var visitor = new DiscountVisitor(isPremiumCustomer: true);

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(49.75m);
            result.Message.Should().Contain("Premium");
        }

        [Fact]
        public void DiscountVisitor_WhenStandardAndSubscription_ShouldReturnZeroDiscount()
        {
            // Arrange — standart müşteriye abonelikte indirim yok
            var product = new SubscriptionProduct("Netflix", 199m, 12);
            var visitor = new DiscountVisitor(isPremiumCustomer: false);

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(0m);
            result.Message.Should().Contain("indirim yok");
        }

        [Fact]
        public void DiscountVisitor_PremiumVsStandard_ShouldAlwaysGiveMoreToPremiun()
        {
            // Arrange
            var physical = new PhysicalProduct("Laptop", 1000m, 2m);
            var digital = new DigitalProduct("Uygulama", 500m, "https://example.com");
            var subscription = new SubscriptionProduct("Spotify", 59m, 6);

            var premium = new DiscountVisitor(isPremiumCustomer: true);
            var standard = new DiscountVisitor(isPremiumCustomer: false);

            // Act & Assert — her tipte premium > standart
            physical.Accept(premium).Amount.Should()
                .BeGreaterThan(physical.Accept(standard).Amount!.Value);

            digital.Accept(premium).Amount.Should()
                .BeGreaterThan(digital.Accept(standard).Amount!.Value);

            subscription.Accept(premium).Amount.Should()
                .BeGreaterThan(subscription.Accept(standard).Amount!.Value);
        }

        // 3. HAPPY PATH — ReportVisitor

        [Fact]
        public void ReportVisitor_WhenPhysicalProduct_ShouldContainKargoLabel()
        {
            // Arrange
            var product = new PhysicalProduct("Laptop", 1000m, 2.5m);
            var visitor = new ReportVisitor();

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ReportLine.Should().Contain("FİZİKSEL");
            result.ReportLine.Should().Contain("Laptop");
            result.ReportLine.Should().Contain("Kargo");
            result.Amount.Should().BeNull();
        }

        [Fact]
        public void ReportVisitor_WhenDigitalProduct_ShouldContainDownloadUrl()
        {
            // Arrange
            var product = new DigitalProduct("Adobe CC", 500m, "https://adobe.com/download");
            var visitor = new ReportVisitor();

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ReportLine.Should().Contain("DİJİTAL");
            result.ReportLine.Should().Contain("https://adobe.com/download");
            result.ReportLine.Should().Contain("Anında Teslimat");
        }

        [Fact]
        public void ReportVisitor_WhenSubscriptionProduct_ShouldContainTotalPrice()
        {
            // Arrange — 59₺ * 6 ay = 354₺ toplam
            var product = new SubscriptionProduct("Spotify", 59m, 6);
            var visitor = new ReportVisitor();

            // Act
            var result = product.Accept(visitor);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ReportLine.Should().Contain("ABONELİK");
            result.ReportLine.Should().Contain("6 ay");
            result.ReportLine.Should().Contain("Spotify");
        }

        // 4. DOUBLE DISPATCH TESTLERİ

        [Fact]
        public void Accept_WhenSameVisitorDifferentProducts_ShouldCallCorrectOverload()
        {
            // Arrange — aynı visitor, farklı ürün tipleri
            var visitor = new TaxCalculatorVisitor();
            var physical = new PhysicalProduct("A", 1000m, 1m);
            var digital = new DigitalProduct("B", 1000m, "https://x.com");
            var subscription = new SubscriptionProduct("C", 1000m, 1);

            // Act
            var physicalTax = physical.Accept(visitor).Amount!.Value;
            var digitalTax = digital.Accept(visitor).Amount!.Value;
            var subscriptionTax = subscription.Accept(visitor).Amount!.Value;

            // Assert — aynı fiyat, farklı vergi oranları
            physicalTax.Should().Be(physicalTax);       // %18+ağırlık vs %8
            digitalTax.Should().Be(digitalTax);  // Dijital daha az vergi
            subscriptionTax.Should().Be(subscriptionTax);     // Abonelik %18 * 1 ay = fiziksel KDV kadar
        }

        [Fact]
        public void Accept_WhenCatalogIterated_ShouldApplyCorrectVisitorPerProduct()
        {
            // Arrange
            var catalog = new List<Visitor_Implementation.Interfaces.IProduct>
        {
            new PhysicalProduct("Laptop", 1000m, 2.5m),
            new DigitalProduct("IDE", 500m, "https://ide.com"),
            new SubscriptionProduct("Netflix", 199m, 12),
        };
            var visitor = new TaxCalculatorVisitor();

            // Act
            var results = catalog.Select(p => p.Accept(visitor)).ToList();

            // Assert — her ürün farklı vergi aldı
            results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
            results.Select(r => r.Amount).Should().OnlyHaveUniqueItems();
        }

        // 5. RESULT NESNESİ TESTLERİ

        [Fact]
        public void VisitResult_Success_ShouldHaveCorrectProperties()
        {
            // Act
            var result = VisitResult.Success(181.25m, "Test mesajı");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(181.25m);
            result.Message.Should().Be("Test mesajı");
            result.ReportLine.Should().BeNull();
        }

        [Fact]
        public void VisitResult_Report_ShouldHaveCorrectProperties()
        {
            // Act
            var result = VisitResult.Report("[FİZİKSEL] Laptop | Fiyat: 1.000₺");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ReportLine.Should().Be("[FİZİKSEL] Laptop | Fiyat: 1.000₺");
            result.Amount.Should().BeNull();
            result.Message.Should().Be("Rapor satırı oluşturuldu.");
        }

        [Fact]
        public void VisitResult_Fail_ShouldHaveCorrectProperties()
        {
            // Act
            var result = VisitResult.Fail("Geçersiz ürün tipi");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Geçersiz ürün tipi");
            result.Amount.Should().BeNull();
            result.ReportLine.Should().BeNull();
        }

        // 6. GUARD CLAUSE TESTLERİ — Ürün Konstruktörleri

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void PhysicalProduct_WhenNameNullOrWhiteSpace_ShouldThrowArgumentException(string? name)
        {
            var act = () => new PhysicalProduct(name!, 1000m, 1m);

            act.Should().Throw<ArgumentException>()
               .WithParameterName("name");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void PhysicalProduct_WhenBasePriceNegativeOrZero_ShouldThrowArgumentOutOfRangeException(decimal price)
        {
            var act = () => new PhysicalProduct("Laptop", price, 1m);

            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithParameterName("basePrice");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void PhysicalProduct_WhenWeightNegativeOrZero_ShouldThrowArgumentOutOfRangeException(decimal weight)
        {
            var act = () => new PhysicalProduct("Laptop", 1000m, weight);

            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithParameterName("weightKg");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void DigitalProduct_WhenNameNullOrWhiteSpace_ShouldThrowArgumentException(string? name)
        {
            var act = () => new DigitalProduct(name!, 500m, "https://x.com");

            act.Should().Throw<ArgumentException>()
               .WithParameterName("name");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void DigitalProduct_WhenDownloadUrlNullOrWhiteSpace_ShouldThrowArgumentException(string? url)
        {
            var act = () => new DigitalProduct("Adobe", 500m, url!);

            act.Should().Throw<ArgumentException>()
               .WithParameterName("downloadUrl");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SubscriptionProduct_WhenDurationNegativeOrZero_ShouldThrowArgumentOutOfRangeException(int months)
        {
            var act = () => new SubscriptionProduct("Netflix", 199m, months);

            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithParameterName("durationMonths");
        }

        // 7. NULL GUARD TESTLERİ — Accept & Visitor

        [Fact]
        public void PhysicalProduct_Accept_WhenVisitorIsNull_ShouldThrowArgumentNullException()
        {
            var product = new PhysicalProduct("Laptop", 1000m, 2.5m);

            var act = () => product.Accept(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("visitor");
        }

        [Fact]
        public void DigitalProduct_Accept_WhenVisitorIsNull_ShouldThrowArgumentNullException()
        {
            var product = new DigitalProduct("Adobe", 500m, "https://adobe.com");

            var act = () => product.Accept(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("visitor");
        }

        [Fact]
        public void SubscriptionProduct_Accept_WhenVisitorIsNull_ShouldThrowArgumentNullException()
        {
            var product = new SubscriptionProduct("Netflix", 199m, 12);

            var act = () => product.Accept(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("visitor");
        }

        [Fact]
        public void TaxVisitor_Visit_WhenPhysicalProductIsNull_ShouldThrowArgumentNullException()
        {
            var visitor = new TaxCalculatorVisitor();

            var act = () => visitor.Visit((PhysicalProduct)null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("product");
        }

        [Fact]
        public void TaxVisitor_Visit_WhenDigitalProductIsNull_ShouldThrowArgumentNullException()
        {
            var visitor = new TaxCalculatorVisitor();

            var act = () => visitor.Visit((DigitalProduct)null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("product");
        }

        [Fact]
        public void TaxVisitor_Visit_WhenSubscriptionProductIsNull_ShouldThrowArgumentNullException()
        {
            var visitor = new TaxCalculatorVisitor();

            var act = () => visitor.Visit((SubscriptionProduct)null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("product");
        }

        [Fact]
        public void DiscountVisitor_Visit_WhenPhysicalProductIsNull_ShouldThrowArgumentNullException()
        {
            var visitor = new DiscountVisitor(isPremiumCustomer: true);

            var act = () => visitor.Visit((PhysicalProduct)null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("product");
        }

        [Fact]
        public void DiscountVisitor_Visit_WhenDigitalProductIsNull_ShouldThrowArgumentNullException()
        {
            var visitor = new DiscountVisitor(isPremiumCustomer: true);

            var act = () => visitor.Visit((DigitalProduct)null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("product");
        }

        [Fact]
        public void DiscountVisitor_Visit_WhenSubscriptionProductIsNull_ShouldThrowArgumentNullException()
        {
            var visitor = new DiscountVisitor(isPremiumCustomer: true);

            var act = () => visitor.Visit((SubscriptionProduct)null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("product");
        }

        [Fact]
        public void ReportVisitor_Visit_WhenPhysicalProductIsNull_ShouldThrowArgumentNullException()
        {
            var visitor = new ReportVisitor();

            var act = () => visitor.Visit((PhysicalProduct)null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("product");
        }

        [Fact]
        public void ReportVisitor_Visit_WhenDigitalProductIsNull_ShouldThrowArgumentNullException()
        {
            var visitor = new ReportVisitor();

            var act = () => visitor.Visit((DigitalProduct)null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("product");
        }

        [Fact]
        public void ReportVisitor_Visit_WhenSubscriptionProductIsNull_ShouldThrowArgumentNullException()
        {
            var visitor = new ReportVisitor();

            var act = () => visitor.Visit((SubscriptionProduct)null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("product");
        }
    }
}