using Bridge_Implementation.Interfaces;
using Bridge_Implementation.Report;
using FluentAssertions;
using Moq;

namespace Bridge_Tests
{
    public class SalesReportTests
    {
        private readonly Mock<IReportRenderer> _rendererMock;
        private readonly SalesReport _sut;

        public SalesReportTests()
        {
            _rendererMock = new Mock<IReportRenderer>();
            _rendererMock.Setup(r => r.RenderName).Returns("MockRenderer");
            _rendererMock.
                Setup(r => r.Render(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>()))
                .Returns("RENDERED_CONTENT");

            _sut = new SalesReport(_rendererMock.Object);
        }

        // --- Generate - Başarılı Senaryo ---

        [Fact]
        public void Generate_ShouldReturnSuccess()
        {
            var result = _sut.Generate();

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Generate_ShouldReturnCorrectReportName()
        {
            var result = _sut.Generate();

            result.ReportName.Should().Be("Satış Raporu");
        }

        [Fact]
        public void Generate_ShouldDelegateToRenderer()
        {
            _sut.Generate();

            _rendererMock.Verify(r => r.Render(
                "Satış Raporu",
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>()),
                Times.Once);
        }

        [Fact]
        public void Generate_ShouldReturnRendererName()
        {
            var result = _sut.Generate();

            result.RendererName.Should().Be("MockRenderer");
        }

        // --- Constructor Testleri ---

        [Fact]
        public void Constructor_WithNullRenderer_ShouldThrowArgumentNullException()
        {
            var act = () => new SalesReport(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("renderer");
        }

        [Fact]
        public void Constructor_WithNegativeTotalSales_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => new SalesReport(_rendererMock.Object, totalSales: -1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Constructor_WithNegativeTotalOrders_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => new SalesReport(_rendererMock.Object, totalOrders: -1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        // --- Bridge Garantisi ---

        [Fact]
        public void SalesReport_ShouldImplement_IReport()
        {
            _sut.Should().BeAssignableTo<IReport>();
        }
    }
}