using Decorator_Implementation.Decorators;
using Decorator_Implementation.Interfaces;
using Decorator_Implementation.Models;
using Decorator_Implementation.Services;
using FluentAssertions;

namespace Decorator_Tests
{
    public class DecoratorChainTests
    {
        private static EmailMessage ValidMessage => new(
                "test@test.com",
                "Test",
                "Test body"
            );

        // --- Tekli Decorator ---

        [Fact]
        public void SingleCompression_ShouldContainCompressionDecorator()
        {
            IEmailService sut = new CompressionEmailDecorator(new EmailService());

            var result = sut.Send(ValidMessage);

            result.IsSuccess.Should().BeTrue();
            result.AppliedDecoratores.Should().Contain("Compression");
        }

        [Fact]
        public void SingleEncryption_ShouldContainEncryptionDecorator()
        {
            IEmailService sut = new EncryptionEmailDecorator(new EmailService());

            var result = sut.Send(ValidMessage);

            result.IsSuccess.Should().BeTrue();
            result.AppliedDecoratores.Should().Contain("Encryption");
        }

        // --- Çift Decorator ---

        [Fact]
        public void CompressionThenEncryption_ShouldContainBothDecorators()
        {
            IEmailService sut = new EncryptionEmailDecorator(
                                new CompressionEmailDecorator(
                                new EmailService()));

            var result = sut.Send(ValidMessage);

            result.IsSuccess.Should().BeTrue();
            result.AppliedDecoratores.Should().Contain("Compression");
            result.AppliedDecoratores.Should().Contain("Encryption");
        }

        // --- Tam Zincir ---

        [Fact]
        public void FullChain_ShouldApplyAllThreeDecorators()
        {
            IEmailService sut = new LoggingEmailDecorator(
                                new EncryptionEmailDecorator(
                                new CompressionEmailDecorator(
                                new EmailService())));

            var result = sut.Send(ValidMessage);

            result.IsSuccess.Should().BeTrue();
            result.AppliedDecoratores.Should().Contain("Compression");
            result.AppliedDecoratores.Should().Contain("Encryption");
            result.AppliedDecoratores.Should().Contain("Logging");
        }

        [Fact]
        public void FullChain_ShouldApplyDecoratorsInCorrectOrder()
        {
            IEmailService sut = new LoggingEmailDecorator(
                                new EncryptionEmailDecorator(
                                new CompressionEmailDecorator(
                                new EmailService())));

            var result = sut.Send(ValidMessage);

            // Sıra: Base -> Compression -> Encryption -> Logging
            result.AppliedDecoratores[0].Should().Be("Base");
            result.AppliedDecoratores[1].Should().Be("Compression");
            result.AppliedDecoratores[2].Should().Be("Encryption");
            result.AppliedDecoratores[3].Should().Be("Logging");
        }

        [Fact]
        public void FullChain_ShouldReturnCorrectRecipient()
        {
            IEmailService sut = new LoggingEmailDecorator(
                                new EncryptionEmailDecorator(
                                new CompressionEmailDecorator(
                                new EmailService())));

            var result = sut.Send(ValidMessage);

            result.To.Should().Be("test@test.com");
        }
    }
}
