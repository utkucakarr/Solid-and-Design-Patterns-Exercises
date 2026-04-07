using Adapter_Implementation.Adapters;
using Adapter_Implementation.ThirdParty;
using FluentAssertions;

namespace Adapter_Tests
{
    public class IyzicoAdapterTests
    {
        private readonly IyzicoAdapter _sut = new(new IyzicoService());

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

            result.ProviderName.Should().Be("Iyzico");
        }

        [Fact]
        public void ProcessPayment_ShouldConvertHttpStatusToBoolean()
        {
            // Adapter HTTP 200 → bool dönüşümü yapıyor
            var result = _sut.ProcessPayment(500, "TRY");

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ProcessPayment_WithZeroAmount_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _sut.ProcessPayment(0, "TRY");

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
        public void Refund_WithEmptyTransactionId_ShouldThrowArgumentException()
        {
            var act = () => _sut.Refund(" ", 500);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_WithNullService_ShouldThrowArgumentNullException()
        {
            var act = () => new IyzicoAdapter(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("iyzicoService");
        }
    }
}
