using FluentAssertions;
using State_Implementation.Interfaces;
using State_Implementation.Models;
using State_Implementation.Orders;
using State_Implementation.States;

namespace State_Tests
{
    public class OrderStateTests
    {
        private readonly IOrderFactory _factory;

        public OrderStateTests()
        {
            _factory = new OrderFactory();
        }

        // HAPPY PATH — Başarılı Geçişler

        [Fact]
        public void Confirm_WhenPending_ShouldTransitionToConfirmed()
        {
            var order = _factory.Create("ORD-001", 1000m);

            var result = order.Confirm();

            result.IsSuccess.Should().BeTrue();
            result.FromState.Should().Be("Ödeme Bekleniyor");
            result.ToState.Should().Be("Sipariş Onaylandı");
            result.OrderId.Should().Be("ORD-001");
            order.GetStatusDescription().Should().Be("Sipariş Onaylandı");
        }

        [Fact]
        public void Ship_WhenConfirmed_ShouldTransitionToShipped()
        {
            var order = _factory.Create("ORD-002", 1000m);
            order.Confirm();

            var result = order.Ship();

            result.IsSuccess.Should().BeTrue();
            result.FromState.Should().Be("Sipariş Onaylandı");
            result.ToState.Should().Be("Kargoya Verildi");
            order.GetStatusDescription().Should().Be("Kargoya Verildi");
        }

        [Fact]
        public void Deliver_WhenShipped_ShouldTransitionToDelivered()
        {
            var order = _factory.Create("ORD-003", 1000m);
            order.Confirm();
            order.Ship();

            var result = order.Deliver();

            result.IsSuccess.Should().BeTrue();
            result.FromState.Should().Be("Kargoya Verildi");
            result.ToState.Should().Be("Teslim Edildi");
            order.GetStatusDescription().Should().Be("Teslim Edildi");
        }

        [Fact]
        public void Cancel_WhenPending_ShouldTransitionToCancelled()
        {
            var order = _factory.Create("ORD-004", 1000m);

            var result = order.Cancel();

            result.IsSuccess.Should().BeTrue();
            result.FromState.Should().Be("Ödeme Bekleniyor");
            result.ToState.Should().Be("İptal Edildi");
            order.GetStatusDescription().Should().Be("İptal Edildi");
        }

        [Fact]
        public void Cancel_WhenConfirmed_ShouldTransitionToCancelled()
        {
            var order = _factory.Create("ORD-005", 1000m);
            order.Confirm();

            var result = order.Cancel();

            result.IsSuccess.Should().BeTrue();
            result.FromState.Should().Be("Sipariş Onaylandı");
            result.ToState.Should().Be("İptal Edildi");
            order.GetStatusDescription().Should().Be("İptal Edildi");
        }

        [Fact]
        public void FullLifecycle_PendingToDelivered_ShouldSucceedAllSteps()
        {
            var order = _factory.Create("ORD-006", 2500m);

            var confirm = order.Confirm();
            var ship = order.Ship();
            var deliver = order.Deliver();

            confirm.IsSuccess.Should().BeTrue();
            ship.IsSuccess.Should().BeTrue();
            deliver.IsSuccess.Should().BeTrue();
            order.GetStatusDescription().Should().Be("Teslim Edildi");
        }

        // BAŞARISIZ SENARYOLAR — Geçersiz Geçişler

        [Fact]
        public void Ship_WhenPending_ShouldFail()
        {
            var order = _factory.Create("ORD-010", 1000m);

            var result = order.Ship();

            result.IsSuccess.Should().BeFalse();
            result.FromState.Should().Be("Ödeme Bekleniyor");
            result.ToState.Should().Be("Ödeme Bekleniyor");
            order.GetStatusDescription().Should().Be("Ödeme Bekleniyor");
        }

        [Fact]
        public void Deliver_WhenPending_ShouldFail()
        {
            var order = _factory.Create("ORD-011", 1000m);

            var result = order.Deliver();

            result.IsSuccess.Should().BeFalse();
            result.FromState.Should().Be("Ödeme Bekleniyor");
            order.GetStatusDescription().Should().Be("Ödeme Bekleniyor");
        }

        [Fact]
        public void Deliver_WhenConfirmed_ShouldFail()
        {
            var order = _factory.Create("ORD-012", 1000m);
            order.Confirm();

            var result = order.Deliver();

            result.IsSuccess.Should().BeFalse();
            result.FromState.Should().Be("Sipariş Onaylandı");
            order.GetStatusDescription().Should().Be("Sipariş Onaylandı");
        }

        [Fact]
        public void Cancel_WhenShipped_ShouldFail()
        {
            var order = _factory.Create("ORD-013", 1000m);
            order.Confirm();
            order.Ship();

            var result = order.Cancel();

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("kargoda");
            order.GetStatusDescription().Should().Be("Kargoya Verildi");
        }

        [Fact]
        public void Cancel_WhenDelivered_ShouldFail()
        {
            var order = _factory.Create("ORD-014", 1000m);
            order.Confirm();
            order.Ship();
            order.Deliver();

            var result = order.Cancel();

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("iptal edilemez");
            order.GetStatusDescription().Should().Be("Teslim Edildi");
        }

        [Fact]
        public void Confirm_WhenAlreadyConfirmed_ShouldFail()
        {
            var order = _factory.Create("ORD-015", 1000m);
            order.Confirm();

            var result = order.Confirm();

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("zaten onaylanmış");
            order.GetStatusDescription().Should().Be("Sipariş Onaylandı");
        }

        [Fact]
        public void Ship_WhenAlreadyShipped_ShouldFail()
        {
            var order = _factory.Create("ORD-016", 1000m);
            order.Confirm();
            order.Ship();

            var result = order.Ship();

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("zaten kargoda");
            order.GetStatusDescription().Should().Be("Kargoya Verildi");
        }

        [Fact]
        public void Deliver_WhenAlreadyDelivered_ShouldFail()
        {
            var order = _factory.Create("ORD-017", 1000m);
            order.Confirm();
            order.Ship();
            order.Deliver();

            var result = order.Deliver();

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("zaten teslim edildi");
            order.GetStatusDescription().Should().Be("Teslim Edildi");
        }

        // CANCELLED STATE — Terminal State Testleri

        [Fact]
        public void Confirm_WhenCancelled_ShouldFail()
        {
            var order = _factory.Create("ORD-020", 1000m);
            order.Cancel();

            var result = order.Confirm();

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("İptal edilmiş");
            order.GetStatusDescription().Should().Be("İptal Edildi");
        }

        [Fact]
        public void Ship_WhenCancelled_ShouldFail()
        {
            var order = _factory.Create("ORD-021", 1000m);
            order.Cancel();

            var result = order.Ship();

            result.IsSuccess.Should().BeFalse();
            order.GetStatusDescription().Should().Be("İptal Edildi");
        }

        [Fact]
        public void Deliver_WhenCancelled_ShouldFail()
        {
            var order = _factory.Create("ORD-022", 1000m);
            order.Cancel();

            var result = order.Deliver();

            result.IsSuccess.Should().BeFalse();
            order.GetStatusDescription().Should().Be("İptal Edildi");
        }

        [Fact]
        public void Cancel_WhenAlreadyCancelled_ShouldFail()
        {
            var order = _factory.Create("ORD-023", 1000m);
            order.Cancel();

            var result = order.Cancel();

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("zaten iptal edilmiş");
            order.GetStatusDescription().Should().Be("İptal Edildi");
        }

        // STATE GEÇİŞ DOĞRULAMA — FromState / ToState

        [Fact]
        public void Confirm_WhenPending_ShouldReturnCorrectStateTransition()
        {
            var order = _factory.Create("ORD-030", 1000m);

            var result = order.Confirm();

            result.FromState.Should().Be("Ödeme Bekleniyor");
            result.ToState.Should().Be("Sipariş Onaylandı");
        }

        [Fact]
        public void Ship_WhenConfirmed_ShouldReturnCorrectStateTransition()
        {
            var order = _factory.Create("ORD-031", 1000m);
            order.Confirm();

            var result = order.Ship();

            result.FromState.Should().Be("Sipariş Onaylandı");
            result.ToState.Should().Be("Kargoya Verildi");
        }

        [Fact]
        public void Deliver_WhenShipped_ShouldReturnCorrectStateTransition()
        {
            var order = _factory.Create("ORD-032", 1000m);
            order.Confirm();
            order.Ship();

            var result = order.Deliver();

            result.FromState.Should().Be("Kargoya Verildi");
            result.ToState.Should().Be("Teslim Edildi");
        }

        [Fact]
        public void FailedTransition_ShouldNotChangeState()
        {
            var order = _factory.Create("ORD-033", 1000m);

            order.Ship(); // Geçersiz

        result: var result = order.Ship();
            result.FromState.Should().Be("Ödeme Bekleniyor");
            result.ToState.Should().Be("Ödeme Bekleniyor");
            order.GetStatusDescription().Should().Be("Ödeme Bekleniyor");
        }

        // GUARD CLAUSE TESTLERİ

        [Fact]
        public void OrderFactory_Create_WhenOrderIdEmpty_ShouldThrowArgumentException()
        {
            var act = () => _factory.Create("", 1000m);

            act.Should().Throw<ArgumentException>()
                .WithParameterName("orderId");
        }

        [Fact]
        public void OrderFactory_Create_WhenOrderIdWhiteSpace_ShouldThrowArgumentException()
        {
            var act = () => _factory.Create("   ", 1000m);

            act.Should().Throw<ArgumentException>()
                .WithParameterName("orderId");
        }

        [Fact]
        public void OrderFactory_Create_WhenAmountZero_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _factory.Create("ORD-999", 0m);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("amount");
        }

        [Fact]
        public void OrderFactory_Create_WhenAmountNegative_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _factory.Create("ORD-999", -100m);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("amount");
        }

        // CONSTRUCTOR NULL GUARD TESTLERİ

        [Fact]
        public void OrderContext_Constructor_WhenOrderIdNull_ShouldThrowArgumentException()
        {
            var act = () => new OrderContext(null!, 1000m, new PendingState());

            act.Should().Throw<ArgumentException>()
                .WithParameterName("orderId");
        }

        [Fact]
        public void OrderContext_Constructor_WhenInitialStateNull_ShouldThrowArgumentNullException()
        {
            var act = () => new OrderContext("ORD-999", 1000m, null!);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("initialState");
        }

        [Fact]
        public void OrderContext_TransitionTo_WhenStateNull_ShouldThrowArgumentNullException()
        {
            var order = _factory.Create("ORD-999", 1000m);

            var act = () => order.TransitionTo(null!);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("newState");
        }

        [Fact]
        public void PendingState_Confirm_WhenContextNull_ShouldThrowArgumentNullException()
        {
            var state = new PendingState();

            var act = () => state.Confirm(null!);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("context");
        }

        [Fact]
        public void ShippedState_Cancel_WhenContextNull_ShouldThrowArgumentNullException()
        {
            var state = new ShippedState();

            var act = () => state.Cancel(null!);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("context");
        }

        // RESULT NESNESİ TESTLERİ

        [Fact]
        public void OrderResult_Success_ShouldHaveCorrectProperties()
        {
            var result = OrderResult.Success("ORD-001", "Test mesajı", "StateA", "StateB");

            result.IsSuccess.Should().BeTrue();
            result.OrderId.Should().Be("ORD-001");
            result.Message.Should().Be("Test mesajı");
            result.FromState.Should().Be("StateA");
            result.ToState.Should().Be("StateB");
        }

        [Fact]
        public void OrderResult_Fail_ShouldHaveCorrectProperties()
        {
            var result = OrderResult.Fail("ORD-001", "Hata mesajı", "StateA");

            result.IsSuccess.Should().BeFalse();
            result.OrderId.Should().Be("ORD-001");
            result.Message.Should().Be("Hata mesajı");
            result.FromState.Should().Be("StateA");
            result.ToState.Should().Be("StateA");
        }

        [Fact]
        public void OrderResult_Success_WhenOrderIdEmpty_ShouldThrowArgumentException()
        {
            var act = () => OrderResult.Success("", "msg", "from", "to");

            act.Should().Throw<ArgumentException>()
                .WithParameterName("orderId");
        }

        [Fact]
        public void OrderResult_Fail_WhenReasonEmpty_ShouldThrowArgumentException()
        {
            var act = () => OrderResult.Fail("ORD-001", "", "StateA");

            act.Should().Throw<ArgumentException>()
                .WithParameterName("reason");
        }

        // BAŞLANGIÇ STATE DOĞRULAMA

        [Fact]
        public void NewOrder_ShouldStartInPendingState()
        {
            var order = _factory.Create("ORD-050", 500m);

            order.GetStatusDescription().Should().Be("Ödeme Bekleniyor");
            order.CurrentState.Should().BeOfType<PendingState>();
        }

        [Fact]
        public void NewOrder_ShouldHaveCorrectOrderIdAndAmount()
        {
            var order = _factory.Create("ORD-051", 1250m);

            order.OrderId.Should().Be("ORD-051");
            order.Amount.Should().Be(1250m);
        }
    }
}