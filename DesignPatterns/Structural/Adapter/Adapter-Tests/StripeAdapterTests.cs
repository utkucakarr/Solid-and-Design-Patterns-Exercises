using Adapter_Implementation.Adapters;
using Adapter_Implementation.ThirdParty;
using FluentAssertions;

namespace Adapter_Tests
{
    public class StripeAdapterTests
    {
        private readonly StripeAdapter _sut = new(new StripeService());

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

            result.ProviderName.Should().Be("Stripe");
        }

        [Fact]
        public void ProcessPayment_ShouldConvertDecimalToCents()
        {
            // Adapter decimal -> cent dönüşümü yapıyor
            var result = _sut.ProcessPayment(25.50m, "TRY");

            result.IsSuccess.Should().BeTrue();
            result.Amount.Should().Be(25.50m);
        }

        [Fact]
        public void ProcessPayment_WithNegativeAmount_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _sut.ProcessPayment(-1, "TRY");

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Refund_ShouldReturnSuccess()
        {
            var paymentResult = _sut.ProcessPayment(1000, "TRY");
            var refundResult = _sut.Refund(paymentResult.TransactionId, 1000);

            refundResult.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithNullService_ShouldThrowArgumentNullException()
        {
            var act = () => new StripeAdapter(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("stripeService");
        }
    }
}
