using Adapter_Implementation.Adapters;
using Adapter_Implementation.ThirdParty;
using FluentAssertions;

namespace Adapter_Tests
{
    public class PayPalAdapterTests
    {
        private readonly PaypalAdapter _sut = new(new PayPalService());

        // --- ProcessPayment Testleri ---
        [Fact]
        public void ProcessPayment_ShouldReturnSuccess()
        {
            var result = _sut.ProcessPayment(1000, "TRY");

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ProcessPayment_ShouldReturnCorrectProviderName()
        {
            var result = _sut.ProcessPayment(1000, "TRY");

            result.ProviderName.Should().Be("PayPal");
        }

        [Fact]
        public void ProcessPayment_ShouldReturnCorrectAmount()
        {
            var result = _sut.ProcessPayment(1000, "TRY");

            result.Amount.Should().Be(1000);
        }

        [Fact]
        public void ProcessPayment_ShouldReturnNonEmptyTransactionId()
        {
            var result = _sut.ProcessPayment(1000, "TRY");

            result.TransactionId.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ProcessPayment_WithZeroAmount_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _sut.ProcessPayment(0, "TRY");

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ProcessPayment_WithEmptyCurrency_ShouldThrowArgumentException()
        {
            var act = () => _sut.ProcessPayment(1000, " ");

            act.Should().Throw<ArgumentException>();
        }

        // --- Refund Testleri ---
        [Fact]
        public void Refund_ShouldReturnSuccess()
        {
            var paymentResult = _sut.ProcessPayment(1000, "TRY");
            var refundResult = _sut.Refund(paymentResult.TransactionId, 1000);

            refundResult.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Refund_WithEmptyTransactionId_ShouldThrowArgumentException()
        {
            var act = () => _sut.Refund(" ", 500);

            act.Should().Throw<ArgumentException>();
        }

        // --- Constructor Testleri ---

        [Fact]
        public void Constructor_WithNullService_ShouldThrowArgumentNullException()
        {
            var act = () => new PaypalAdapter(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("payPalService");
        }
    }
}