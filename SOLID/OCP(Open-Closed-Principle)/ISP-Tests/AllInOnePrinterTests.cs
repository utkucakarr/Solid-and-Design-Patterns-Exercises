using FluentAssertions;
using ISP_Implementation.Interfaces;
using ISP_Implementation.Printers;

namespace ISP_Tests
{
    public class AllInOnePrinterTests
    {
        private readonly AllInOnePrinter _allInOnePrinter = new();

        // ─── IPrintable ──

        [Fact]
        public void Print_ShouldReturnSuccess()
        {
            var result = _allInOnePrinter.Print("Rapor.pdf");

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Print_WithEmptyDocument_ShouldThrowArgumentException()
        {
            var act = () => _allInOnePrinter.Print(" ");

            act.Should().Throw<ArgumentException>();
        }

        // ─── IScannable ──

        [Fact]
        public void Scan_ShouldReturnSuccess()
        {
            var result = _allInOnePrinter.Scan("Sozlesme.pdf");

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Scan_WithEmptyDocument_ShouldThrowArgumentException()
        {
            var act = () => _allInOnePrinter.Scan(" ");

            act.Should().Throw<ArgumentException>();
        }

        // ─── IFaxable ──

        [Fact]
        public void Fax_ShouldReturnSuccess()
        {
            var result = _allInOnePrinter.Fax("Fatura.pdf");

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Fax_ShouldContainDocumentName_InMessage()
        {
            var result = _allInOnePrinter.Fax("Fatura.pdf");

            result.Message.Should().Contain("Fatura.pdf");
        }

        [Fact]
        public void Fax_WithEmptyDocument_ShouldThrowArgumentException()
        {
            var act = () => _allInOnePrinter.Fax(" ");

            act.Should().Throw<ArgumentException>();
        }

        // ─── ISP Garantisi ──

        [Fact]
        public void AllInOne_ShouldBeAssignableTo_IPrintable()
        {
            _allInOnePrinter.Should().BeAssignableTo<IPrintable>();
        }

        [Fact]
        public void AllInOne_ShouldBeAssignableTo_IScannable()
        {
            _allInOnePrinter.Should().BeAssignableTo<IScannable>();
        }

        [Fact]
        public void AllInOne_ShouldBeAssignableTo_IFaxable()
        {
            _allInOnePrinter.Should().BeAssignableTo<IFaxable>();
        }
    }
}
