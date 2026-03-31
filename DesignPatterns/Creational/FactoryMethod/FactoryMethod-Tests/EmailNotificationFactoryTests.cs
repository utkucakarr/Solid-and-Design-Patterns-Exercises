using FactoryMethod_Implementation.Factories;
using FactoryMethod_Implementation.Interfaces;
using FactoryMethod_Implementation.Services;
using FluentAssertions;

namespace FactoryMethod_Tests
{
    public class EmailNotificationFactoryTests
    {
        private readonly EmailNotificationFactory _sut = new();

        [Fact]
        public void CreateNotification_ShouldReturnEmailNotification()
        {
            var notification = _sut.CreateNotification();

            notification.Should().BeOfType<EmailNotification>();
        }

        [Fact]
        public void CreateNotification_ShouldReturnINotification()
        {
            var notification = _sut.CreateNotification();

            notification.Should().BeAssignableTo<INotification>();
        }

        [Fact]
        public void CreateNotification_ShouldReturnNewInstance_EachTime()
        {
            var notification1 = _sut.CreateNotification();
            var notification2 = _sut.CreateNotification();

            // Factory her seferinde yeni instance üretiyor
            notification1.Should().NotBeSameAs(notification2);
        }

        [Fact]
        public void CreateNotification_ShouldReturnCorrectChannel()
        {
            var notification = _sut.CreateNotification();

            notification.Channel.Should().Be("Email");
        }

        // --- Send Testleri ---

        [Fact]
        public void Send_ShouldReturnSuccess()
        {
            var notification = _sut.CreateNotification();

            var result = notification.Send("utku@example.com", "Test mesajý");

            result.Message.Should().Contain("utku@example.com");
        }

        [Fact]
        public void Send_ShouldContainRecipient_InMessage()
        {
            var notification = _sut.CreateNotification();

            var result = notification.Send("utku@example.com", "Test mesajý");

            result.Message.Should().Contain("utku@example.com");
        }

        [Fact]
        public void Send_WithEmptyRecipient_ShouldThrowArgumentException()
        {
            var notification = _sut.CreateNotification();

            var act = () => notification.Send(" ", "Test mesajý");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Send_WithEmptyMessage_ShouldThrowArgumentException()
        {
            var notification = _sut.CreateNotification();

            var act = () => notification.Send("utku@example.com", " ");

            act.Should().Throw<ArgumentException>();
        }
    }
}