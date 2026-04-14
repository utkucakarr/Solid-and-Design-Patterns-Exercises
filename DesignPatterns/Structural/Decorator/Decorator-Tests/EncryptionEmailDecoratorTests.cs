using Decorator_Implementation.Decorators;
using Decorator_Implementation.Interfaces;
using Decorator_Implementation.Models;
using FluentAssertions;
using Moq;

namespace Decorator_Tests
{
    public class EncryptionEmailDecoratorTests
    {
        private readonly Mock<IEmailService> _innerServiceMock;
        private readonly EncryptionEmailDecorator _sut;

        private static EmailMessage ValidMessage => new(
            "test @test.com",
            "Test",
            "Test body"
            );

        public EncryptionEmailDecoratorTests()
        {
            _innerServiceMock = new Mock<IEmailService>();
            _innerServiceMock
                .Setup(s => s.Send(It.IsAny<EmailMessage>()))
                .Returns((EmailMessage m) =>
                    EmailResult.Success(m.To, m.Subject, new List<string> { "Base" }));

            _sut = new EncryptionEmailDecorator(_innerServiceMock.Object);
        }

        // --- Başarılı Senaryo ---

        [Fact]
        public void Send_ShouldReturnSuccess()
        {
            var result = _sut.Send(ValidMessage);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Send_ShouldDelegateToInnerService()
        {
            _sut.Send(ValidMessage);

            _innerServiceMock.Verify(s => s.Send(It.IsAny<EmailMessage>()), Times.Once);
        }

        [Fact]
        public void Send_ShouldEncryptBody_BeforeCallingInnerService()
        {
            string? capturedBody = null;
            _innerServiceMock
                .Setup(s => s.Send(It.IsAny<EmailMessage>()))
                .Callback<EmailMessage>(m => capturedBody = m.Body)
                .Returns((EmailMessage m) =>
                    EmailResult.Success(m.To, m.Subject, new List<string> { "Base" }));

            _sut.Send(ValidMessage);

            // Inner servise giden body şifrelenmiş olmalı — Base64
            capturedBody.Should().NotBe("Test body");
            var decoded = System.Text.Encoding.UTF8.GetString(
                              Convert.FromBase64String(capturedBody!));
            decoded.Should().Be("Test body");
        }

        [Fact]
        public void Send_ShouldAddEncryptionToAppliedDecorators()
        {
            var result = _sut.Send(ValidMessage);

            result.AppliedDecoratores.Should().Contain("Encryption");
        }

        // --- Constructor Testleri ---

        [Fact]
        public void Constructor_WithNullInnerService_ShouldThrowArgumentNullException()
        {
            var act = () => new EncryptionEmailDecorator(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("innerService");
        }

        [Fact]
        public void EncryptionEmailDecorator_ShouldImplement_IEmailService()
        {
            _sut.Should().BeAssignableTo<IEmailService>();
        }
    }
}
