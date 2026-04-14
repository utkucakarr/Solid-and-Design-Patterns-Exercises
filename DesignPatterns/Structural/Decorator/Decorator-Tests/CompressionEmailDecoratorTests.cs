using Decorator_Implementation.Decorators;
using Decorator_Implementation.Interfaces;
using Decorator_Implementation.Models;
using FluentAssertions;
using Moq;

namespace Decorator_Tests
{
    public class CompressionEmailDecoratorTests
    {
        private readonly Mock<IEmailService> _innerServiceMock;
        private readonly CompressionEmailDecorator _sut;

        private static EmailMessage ValidMessage => new(
                "test@test.com",
                "Test",
                "Test body"
            );

        public CompressionEmailDecoratorTests()
        {
            _innerServiceMock = new Mock<IEmailService>();
            _innerServiceMock
                .Setup(s => s.Send(It.IsAny<EmailMessage>()))
                .Returns((EmailMessage m) =>
                    EmailResult.Success(m.To, m.Subject, new List<string> { "Base" }));

            _sut = new CompressionEmailDecorator(_innerServiceMock.Object);
        }

        // --- Başarılı Senaryo (Successful scenario)
        [Fact]
        public void Send_ShouldReturnSuccess()
        {
            var result = _sut.Send(ValidMessage);

            result.IsSuccess.Should().BeTrue();
        }
        [Fact]
        public void Send_ShouldCompressBody_BeforeCallingInnerService()
        {
            string? capturedBody = null;
            _innerServiceMock
                .Setup(s => s.Send(It.IsAny<EmailMessage>()))
                .Callback<EmailMessage>(m => capturedBody = m.Body)
                .Returns((EmailMessage m) =>
                    EmailResult.Success(m.To, m.Subject, new List<string> { "Base" }));

            _sut.Send(ValidMessage);

            // Inner servise giden body sıkıştırılmış olmalı
            capturedBody.Should().Contain("[COMPRESSED]");
        }

        [Fact]
        public void Send_ShouldAddCompressionToAppliedDecorators()
        {
            var result = _sut.Send(ValidMessage);

            result.AppliedDecoratores.Should().Contain("Compression");
        }

        // --- Constructor Testleri ---

        [Fact]
        public void Constructor_WithNullInnerService_ShouldThrowArgumentNullException()
        {
            var act = () => new CompressionEmailDecorator(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("innerService");
        }

        // --- Decorator Garantisi ---

        [Fact]
        public void CompressionEmailDecorator_ShouldImplement_IEmailService()
        {
            _sut.Should().BeAssignableTo<IEmailService>();
        }
    }
}