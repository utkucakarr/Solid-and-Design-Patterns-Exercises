using FluentAssertions;
using Moq;
using Strategy_Implementation.Context;
using Strategy_Implementation.Interfaces;
using Strategy_Implementation.Models;
using Strategy_Implementation.Strategies;

namespace Strategy_Tests
{
    public class ShippingStrategyTests
    {
        // HAPPY PATH — StandardShippingStrategy

        [Fact]
        public void Calculate_ValidOrder_ShouldReturnCorrectStandardCost()
        {
            var strategy = new StandardShippingStrategy();
            var order = new ShippingOrder { OrderId = "ORD-001", WeightKg = 3.5, OrderTotal = 200m, MembershipType = "standard" };

            var result = strategy.Calculate(order);

            // BaseFee(15) + WeightKg(3.5) * WeightRate(2.5) = 15 + 8.75 = 23.75
            result.IsSuccess.Should().BeTrue();
            result.Cost.Should().Be(23.75m);
            result.CarrierName.Should().Be("Mng Kargo");
            result.EstimatedDays.Should().Be(3);
            result.StrategyUsed.Should().Be("Standart Kargo");
        }

        // HAPPY PATH — ExpressShippingStrategy

        [Fact]
        public void Calculate_ValidOrder_ShouldReturnCorrectExpressCost()
        {
            var strategy = new ExpressingShippingStrategy();
            var order = new ShippingOrder { OrderId = "ORD-002", WeightKg = 2.0, OrderTotal = 300m, MembershipType = "standard" };

            var result = strategy.Calculate(order);

            // BaseFee(35) + WeightKg(2) * WeightRate(5) = 35 + 10 = 45
            result.IsSuccess.Should().BeTrue();
            result.Cost.Should().Be(45.00m);
            result.CarrierName.Should().Be("Yurtiçi Kargo");
            result.EstimatedDays.Should().Be(1);
            result.StrategyUsed.Should().Be("Hızlı Kargo");
        }

        // HAPPY PATH — FreeShippingStrategy

        [Fact]
        public void Calculate_OrderAboveThreshold_ShouldReturnZeroCost()
        {
            var strategy = new FreeShippingStrategy();
            var order = new ShippingOrder { OrderId = "ORD-003", WeightKg = 5.0, OrderTotal = 600m, MembershipType = "standard" };

            var result = strategy.Calculate(order);

            result.IsSuccess.Should().BeTrue();
            result.Cost.Should().Be(0m);
            result.CarrierName.Should().Be("Aras Kargo");
            result.EstimatedDays.Should().Be(5);
        }

        // HAPPY PATH — MemberShippingStrategy

        [Fact]
        public void Calculate_PremiumMember_ShouldApplyDiscount()
        {
            var strategy = new MemberShippingStrategy();
            var order = new ShippingOrder { OrderId = "ORD-004", WeightKg = 4.0, OrderTotal = 400m, MembershipType = "Premium" };

            var result = strategy.Calculate(order);

            // BaseFee(15) + 4*2.5=10 → total=25 → %40 indirim → 25*0.6 = 15
            result.IsSuccess.Should().BeTrue();
            result.Cost.Should().Be(25.00m);
            result.CarrierName.Should().Be("PTT Kargo");
            result.EstimatedDays.Should().Be(4);
        }

        // BAŞARISIZ SENARYO — FreeShipping eşik altı

        [Fact]
        public void Calculate_OrderBelowFreeThreshold_ShouldFail()
        {
            var strategy = new FreeShippingStrategy();
            var order = new ShippingOrder { OrderId = "ORD-005", WeightKg = 1.0, OrderTotal = 200m, MembershipType = "standard" };

            var result = strategy.Calculate(order);

            result.IsSuccess.Should().BeFalse();
            result.Cost.Should().Be(0m);
            result.Message.Should().Contain("500");
        }

        // BAŞARISIZ SENARYO — Standart üye, member stratejisi

        [Fact]
        public void Calculate_NonPremiumMember_ShouldFailMemberStrategy()
        {
            var strategy = new MemberShippingStrategy();
            var order = new ShippingOrder { OrderId = "ORD-006", WeightKg = 2.0, OrderTotal = 300m, MembershipType = "standard" };

            var result = strategy.Calculate(order);

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Premium");
        }

        // CONTEXT — Strateji değişimi (Moq ile)

        [Fact]
        public void ExecuteShipping_WhenStrategySet_ShouldDelegateToStrategy()
        {
            var mockStrategy = new Mock<IShippingStrategy>();
            var order = new ShippingOrder { OrderId = "ORD-007", WeightKg = 2.0, OrderTotal = 300m, MembershipType = "standard" };
            var expected = ShippingResult.Success(25m, 2, "TestCarrier", "TestStrategy");

            mockStrategy.Setup(s => s.Calculate(order)).Returns(expected);

            var context = new ShippingContext();
            context.SetStrategy(mockStrategy.Object);
            var result = context.ExecuteShipping(order);

            result.Should().Be(expected);
            mockStrategy.Verify(s => s.Calculate(order), Times.Once);
        }

        [Fact]
        public void ExecuteShipping_WhenStrategyChanged_ShouldUseNewStrategy()
        {
            var firstStrategy = new Mock<IShippingStrategy>();
            var secondStrategy = new Mock<IShippingStrategy>();
            var order = new ShippingOrder { OrderId = "ORD-008", WeightKg = 2.0, OrderTotal = 300m, MembershipType = "standard" };

            firstStrategy.Setup(s => s.Calculate(order)).Returns(ShippingResult.Success(30m, 3, "A", "First"));
            secondStrategy.Setup(s => s.Calculate(order)).Returns(ShippingResult.Success(0m, 5, "B", "Second"));

            var context = new ShippingContext();

            context.SetStrategy(firstStrategy.Object);
            context.ExecuteShipping(order);

            context.SetStrategy(secondStrategy.Object);
            var result = context.ExecuteShipping(order);

            result.Cost.Should().Be(0m);
            firstStrategy.Verify(s => s.Calculate(order), Times.Once);
            secondStrategy.Verify(s => s.Calculate(order), Times.Once);
        }

        // CONTEXT — Strateji belirlenmemiş

        [Fact]
        public void ExecuteShipping_WhenNoStrategySet_ShouldReturnFail()
        {
            var context = new ShippingContext();
            var order = new ShippingOrder { OrderId = "ORD-009", WeightKg = 2.0, OrderTotal = 300m, MembershipType = "standard" };

            var result = context.ExecuteShipping(order);

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("SetStrategy");
        }

        // GUARD CLAUSE — Null order

        [Theory]
        [InlineData(typeof(StandardShippingStrategy))]
        [InlineData(typeof(ExpressingShippingStrategy))]
        [InlineData(typeof(FreeShippingStrategy))]
        [InlineData(typeof(MemberShippingStrategy))]
        public void Calculate_NullOrder_ShouldThrowArgumentNullException(Type strategyType)
        {
            var strategy = (IShippingStrategy)Activator.CreateInstance(strategyType)!;

            var act = () => strategy.Calculate(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("order");
        }

        // GUARD CLAUSE — Boş OrderId

        [Theory]
        [InlineData(typeof(StandardShippingStrategy))]
        [InlineData(typeof(ExpressingShippingStrategy))]
        [InlineData(typeof(FreeShippingStrategy))]
        [InlineData(typeof(MemberShippingStrategy))]
        public void Calculate_EmptyOrderId_ShouldThrowArgumentException(Type strategyType)
        {
            var strategy = (IShippingStrategy)Activator.CreateInstance(strategyType)!;
            var order = new ShippingOrder { OrderId = "", WeightKg = 2.0, OrderTotal = 300m, MembershipType = "standard" };

            var act = () => strategy.Calculate(order);

            act.Should().Throw<ArgumentException>()
               .WithParameterName("OrderId");
        }

        // GUARD CLAUSE — Sıfır ağırlık

        [Theory]
        [InlineData(typeof(StandardShippingStrategy))]
        [InlineData(typeof(ExpressingShippingStrategy))]
        [InlineData(typeof(FreeShippingStrategy))]
        [InlineData(typeof(MemberShippingStrategy))]
        public void Calculate_ZeroWeight_ShouldThrowArgumentOutOfRangeException(Type strategyType)
        {
            var strategy = (IShippingStrategy)Activator.CreateInstance(strategyType)!;
            var order = new ShippingOrder { OrderId = "ORD-X", WeightKg = 0, OrderTotal = 300m, MembershipType = "standard" };

            var act = () => strategy.Calculate(order);

            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithParameterName("WeightKg");
        }

        // GUARD CLAUSE — Context null strateji

        [Fact]
        public void SetStrategy_NullStrategy_ShouldThrowArgumentNullException()
        {
            var context = new ShippingContext();

            var act = () => context.SetStrategy(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("strategy");
        }

        // GUARD CLAUSE — Context null order

        [Fact]
        public void ExecuteShipping_NullOrder_ShouldThrowArgumentNullException()
        {
            var context = new ShippingContext();
            context.SetStrategy(new StandardShippingStrategy());

            var act = () => context.ExecuteShipping(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("order");
        }

        // SENARYO — Flash sale runtime strateji değişimi

        [Fact]
        public void ExecuteShipping_FlashSaleScenario_ShouldSwitchToFreeShipping()
        {
            var context = new ShippingContext();
            var order = new ShippingOrder { OrderId = "ORD-FLASH", WeightKg = 2.0, OrderTotal = 1200m, MembershipType = "standard" };

            context.SetStrategy(new StandardShippingStrategy());
            var normalResult = context.ExecuteShipping(order);

            context.SetStrategy(new FreeShippingStrategy());
            var flashResult = context.ExecuteShipping(order);

            normalResult.Cost.Should().BeGreaterThan(0m);
            flashResult.Cost.Should().Be(0m);
            flashResult.StrategyUsed.Should().Be("Ücretsiz Kargo");
        }
    }
}