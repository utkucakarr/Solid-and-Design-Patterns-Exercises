using Decorator_Implementation.Decorators;
using Decorator_Implementation.Interfaces;
using Decorator_Implementation.Models;
using FluentAssertions;
using Moq;

namespace Decorator_Tests
{
    public class LoggingEmailDecoratorTests
    {
        private readonly Mock<IEmailService> _innerServiceMock;
        private readonly LoggingEmailDecorator _sut;

        private static EmailMessage ValidMessage => new(
            "test @test.com",
            "Test",
            "Test body"
            );

        public LoggingEmailDecoratorTests()
        {
            _innerServiceMock = new Mock<IEmailService>();
            _innerServiceMock
                .Setup(s => s.Send(It.IsAny<EmailMessage>()))
                .Returns((EmailMessage m) =>
                    EmailResult.Success(m.To, m.Subject, new List<string> { "Base" }));

            _sut = new LoggingEmailDecorator(_innerServiceMock.Object);
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
        public void Send_ShouldAddLoggingToAppliedDecorators()
        {
            var result = _sut.Send(ValidMessage);

            result.AppliedDecoratores.Should().Contain("Logging");
        }

        [Fact]
        public void Send_WhenInnerServiceFails_ShouldReturnFail()
        {
            _innerServiceMock
                .Setup(s => s.Send(It.IsAny<EmailMessage>()))
                .Returns(EmailResult.Fail("test@test.com", "SMTP hatası"));

            var result = _sut.Send(ValidMessage);

            result.IsSuccess.Should().BeFalse();
        }

        // --- Constructor Testleri ---

        [Fact]
        public void Constructor_WithNullInnerService_ShouldThrowArgumentNullException()
        {
            var act = () => new LoggingEmailDecorator(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("innerService");
        }

        [Fact]
        public void LoggingEmailDecorator_ShouldImplement_IEmailService()
        {
            _sut.Should().BeAssignableTo<IEmailService>();
        }
    }
}
