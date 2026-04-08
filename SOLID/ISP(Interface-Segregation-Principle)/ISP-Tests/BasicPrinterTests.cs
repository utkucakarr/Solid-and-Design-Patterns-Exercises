using FluentAssertions;
using ISP_Implementation.Interfaces;
using ISP_Implementation.Printers;

namespace ISP_Tests
{
    public class BasicPrinterTests
    {
        private readonly BasicPrinter _basicPrinter = new();

        [Fact]
        public void Print_ShouldReturnSuccess()
        {
            var result = _basicPrinter.Print("Rapor.pdf");

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Print_ShouldContainDocumentName_InMessage()
        {
            var result = _basicPrinter.Print("Rapor.pdf");

            result.Message.Should().Contain("Rapor.pdf");
        }

        [Fact]
        public void Print_WithEmptyDocument_ShouldThrowArgumentException()
        {
            var act = () => _basicPrinter.Print(" ");

            act.Should().Throw<ArgumentException>();
        }

        // --- ISP Garantisi ---

        [Fact]
        public void BasicPrinter_ShouldBeAssignableTo_IPrintable()
        {
            _basicPrinter.Should().BeAssignableTo<IPrintable>();
        }

        [Fact]
        public void BasicPrinter_ShouldNotBeAssignableTo_IScannable()
        {
            // BasicPrinter tarayamaz — tip sistemi bunu garanti eder
            _basicPrinter.Should().NotBeAssignableTo<IScannable>();
        }

        [Fact]
        public void BasicPrinter_ShouldNotBeAssignableTo_IFaxable()
        {
            // BasicPrinter faks gönderemez — tip sistemi bunu garanti eder
            _basicPrinter.Should().NotBeAssignableTo<IFaxable>();
        }

        [Fact]
        public void BasicPrinter_AsIPrintable_ShouldNotThrow()
        {
            // ISP: IPrintable referansýyla kullanýldýðýnda exception yok!
            IPrintable printer = new BasicPrinter();

            var act = () => printer.Print("Test.pdf");

            act.Should().NotThrow();
        }
    }
}