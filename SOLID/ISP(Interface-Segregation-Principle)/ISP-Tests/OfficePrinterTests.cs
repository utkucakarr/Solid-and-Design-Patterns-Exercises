using FluentAssertions;
using ISP_Implementation.Interfaces;
using ISP_Implementation.Printers;

namespace ISP_Tests
{
    public class OfficePrinterTests
    {
        private readonly OfficePrinter _officePrinter = new();

        // --- IPrintable ---

        [Fact]
        public void Print_ShouldReturnSuccess()
        {
            var result = _officePrinter.Print("Rapor.pdf");

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Print_WithEmptyDocument_ShouldThrowArgumentException()
        {
            var act = () => _officePrinter.Print(" ");

            act.Should().Throw<ArgumentException>();
        }

        // --- IScannable ---

        [Fact]
        public void Scan_ShouldReturnSuccess()
        {
            var result = _officePrinter.Scan("Sozlesme.pdf");

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Scan_ShouldContainDocumentName_InMessage()
        {
            var result = _officePrinter.Scan("Sozlesme.pdf");

            result.Message.Should().Contain("Sozlesme.pdf");
        }

        [Fact]
        public void Scan_WithEmptyDocument_ShouldThrowArgumentException()
        {
            var act = () => _officePrinter.Scan(" ");

            act.Should().Throw<ArgumentException>();
        }

        // --- ISP Garantisi ---

        [Fact]
        public void OfficePrinter_ShouldBeAssignableTo_IPrintable()
        {
            _officePrinter.Should().BeAssignableTo<IPrintable>();
        }

        [Fact]
        public void OfficePrinter_ShouldBeAssignableTo_IScannable()
        {
            _officePrinter.Should().BeAssignableTo<IScannable>();
        }

        [Fact]
        public void OfficePrinter_ShouldNotBeAssignableTo_IFaxable()
        {
            // OfficePrinter faks gönderemez — tip sistemi bunu garanti eder
            _officePrinter.Should().NotBeAssignableTo<IFaxable>();
        }
    }
}
