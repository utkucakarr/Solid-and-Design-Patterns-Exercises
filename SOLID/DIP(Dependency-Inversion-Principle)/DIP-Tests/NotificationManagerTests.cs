using DIP_Implementation.Interfaces;
using DIP_Implementation.Models;
using DIP_Implementation.Services;
using FluentAssertions;
using Moq;

namespace DIP_Tests
{
    public class NotificationManagerTests
    {
        // --- Constructor Testleri ---

        [Fact]
        public void Constructor_WithNullServices_ShouldThrowArgumentNullException()
        {
            var act = () => new NotificationManager(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("services");
        }

        // --- SendAll Testleri ---

        [Fact]
        public void SendAll_ShouldCallSend_OnAllServices()
        {
            // Arrange
            var mockEmail = new Mock<INotificationService>();
            var mockSms = new Mock<INotificationService>();

            mockEmail.Setup(s => s.Send(It.IsAny<string>()))
                     .Returns(NotificationResult.Success("EMAIL", "test"));
            mockSms.Setup(s => s.Send(It.IsAny<string>()))
                   .Returns(NotificationResult.Success("SMS", "test"));

            var manager = new NotificationManager(new[]
            {
            mockEmail.Object,
            mockSms.Object
        });

            // Act
            manager.SendAll("Test mesajı");

            // Assert
            mockEmail.Verify(s => s.Send("Test mesajı"), Times.Once);
            mockSms.Verify(s => s.Send("Test mesajı"), Times.Once);
        }

        [Fact]
        public void SendAll_ShouldReturnSuccessResults_ForAllServices()
        {
            // Arrange
            var mockEmail = new Mock<INotificationService>();
            var mockSms = new Mock<INotificationService>();

            mockEmail.Setup(s => s.Send(It.IsAny<string>()))
                     .Returns(NotificationResult.Success("EMAIL", "test"));
            mockSms.Setup(s => s.Send(It.IsAny<string>()))
                   .Returns(NotificationResult.Success("SMS", "test"));

            var manager = new NotificationManager(new[]
            {
            mockEmail.Object,
            mockSms.Object
        });

            // Act
            var results = manager.SendAll("Test mesajı").ToList();

            // Assert
            results.Should().HaveCount(2);
            results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
        }

        [Fact]
        public void SendAll_WithEmptyMessage_ShouldThrowArgumentException()
        {
            var manager = new NotificationManager(new List<INotificationService>());

            var act = () => manager.SendAll(" ").ToList();

            act.Should().Throw<ArgumentException>();
        }

        // --- SendToChannel Testleri ---

        [Fact]
        public void SendToChannel_ShouldCallCorrectService()
        {
            // Arrange
            var mockEmail = new Mock<INotificationService>();
            var mockSms = new Mock<INotificationService>();

            mockEmail.Setup(s => s.Channel).Returns("EMAIL");
            mockSms.Setup(s => s.Channel).Returns("SMS");

            mockSms.Setup(s => s.Send(It.IsAny<string>()))
                   .Returns(NotificationResult.Success("SMS", "test"));

            var manager = new NotificationManager(new[]
            {
            mockEmail.Object,
            mockSms.Object
        });

            // Act
            manager.SendToChannel("SMS", "Test mesajı");

            // Assert — sadece SMS servisi çağrıldı
            mockSms.Verify(s => s.Send("Test mesajı"), Times.Once);
            mockEmail.Verify(s => s.Send(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void SendToChannel_WithUnknownChannel_ShouldReturnFailResult()
        {
            var manager = new NotificationManager(new List<INotificationService>());

            var result = manager.SendToChannel("WHATSAPP", "Test mesajı");

            result!.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Kanal bulunamadı.");
        }

        [Fact]
        public void SendToChannel_WithEmptyChannel_ShouldThrowArgumentException()
        {
            var manager = new NotificationManager(new List<INotificationService>());

            var act = () => manager.SendToChannel(" ", "Test mesajı");

            act.Should().Throw<ArgumentException>();
        }

        // --- DIP Garantisi ---

        [Fact]
        public void NotificationManager_ShouldWork_WithAnyINotificationService()
        {
            // DIP: NotificationManager hangi implementasyon gelirse gelsin çalışmalı
            var mockService = new Mock<INotificationService>();

            mockService.Setup(s => s.Channel).Returns("CUSTOM");
            mockService.Setup(s => s.Send(It.IsAny<string>()))
                       .Returns(NotificationResult.Success("CUSTOM", "test"));

            var manager = new NotificationManager(new[] { mockService.Object });

            var results = manager.SendAll("Test").ToList();

            results.Should().HaveCount(1);
            results[0].IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void NotificationManager_ShouldWork_WithDifferentNumberOfServices()
        {
            // DIP: 1 servis de 10 servis de — NotificationManager değişmez
            var services = Enumerable.Range(1, 5)
                .Select(i =>
                {
                    var mock = new Mock<INotificationService>();
                    mock.Setup(s => s.Channel).Returns($"CHANNEL_{i}");
                    mock.Setup(s => s.Send(It.IsAny<string>()))
                        .Returns(NotificationResult.Success($"CHANNEL_{i}", "test"));
                    return mock.Object;
                });

            var manager = new NotificationManager(services);

            var results = manager.SendAll("Test").ToList();

            results.Should().HaveCount(5);
        }
    }
}