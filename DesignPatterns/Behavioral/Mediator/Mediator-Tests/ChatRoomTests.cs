using FluentAssertions;
using Mediator_Implementation.Colleagues;
using Mediator_Implementation.Interfaces;
using Mediator_Implementation.Mediator;
using Mediator_Implementation.Models;
using Moq;

namespace Mediator_Tests
{
    public class ChatRoomTests
    {
        #region Helpers

        private static ChatRoom CreateRoom(string name = "Test Odası") => new(name);

        private static RegularUser CreateRegularUser(string username = "testuser") =>
            new(username);

        private static AdminUser CreateAdminUser(string username = "testadmin") =>
            new(username);

        private static BotUser CreateBotUser(string username = "testbot") =>
            new(username);

        private static (ChatRoom room, AdminUser admin,
            RegularUser user1, RegularUser user2) CreatePopulatedRoom()
        {
            var room = CreateRoom();
            var admin = CreateAdminUser("admin");
            var user1 = CreateRegularUser("user1");
            var user2 = CreateRegularUser("user2");

            room.Register(admin);
            room.Register(user1);
            room.Register(user2);

            return (room, admin, user1, user2);
        }

        #endregion

        [Fact]
        public void Register_WithValidUser_ShouldAddUserToRoom()
        {
            // Arrange
            var room = CreateRoom();
            var user = CreateRegularUser();

            // Act
            room.Register(user);

            // Assert
            room.GetActiveUsers().Should().ContainSingle(u => u.Username == "testuser");
        }

        [Fact]
        public void Register_ShouldSetMediatorOnUser()
        {
            // Arrange
            var room = CreateRoom();
            var mockUser = new Mock<IUser>();
            mockUser.Setup(u => u.Username).Returns("mockuser");

            // Act
            room.Register(mockUser.Object);

            // Assert
            mockUser.Verify(u => u.SetMediator(room), Times.Once);
        }

        [Fact]
        public void Register_SamUserTwice_ShouldNotDuplicate()
        {
            // Arrange
            var room = CreateRoom();
            var user = CreateRegularUser();

            // Act
            room.Register(user);
            room.Register(user);

            // Assert
            room.GetActiveUsers().Should().ContainSingle(u => u.Username == "testuser");
        }

        [Fact]
        public void Unregister_WithRegisteredUser_ShouldRemoveFromRoom()
        {
            // Arrange
            var room = CreateRoom();
            var user = CreateRegularUser();
            room.Register(user);

            // Act
            room.Unregister(user);

            // Assert
            room.GetActiveUsers().Should().BeEmpty();
        }

        [Fact]
        public void Unregister_WithUnregisteredUser_ShouldNotThrow()
        {
            // Arrange
            var room = CreateRoom();
            var user = CreateRegularUser();

            // Act
            var act = () => room.Unregister(user);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void GetActiveUsers_ShouldReturnAllRegisteredUsers()
        {
            // Arrange
            var (room, admin, user1, user2) = CreatePopulatedRoom();

            // Act
            var activeUsers = room.GetActiveUsers();

            // Assert
            activeUsers.Should().HaveCount(3);
            activeUsers.Select(u => u.Username)
                .Should().Contain(new[] { "admin", "user1", "user2" });
        }

        // SEND MESSAGE — Public mesaj testleri

        [Fact]
        public void SendMessage_WithValidSender_ShouldReturnSuccess()
        {
            // Arrange
            var (room, _, user1, _) = CreatePopulatedRoom();

            // Act
            var result = room.SendMessage("user1", "Merhaba!");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.SenderUsername.Should().Be("user1");
            result.MessageType.Should().Be(MessageType.Public);
        }

        [Fact]
        public void SendMessage_ShouldDeliverToAllUsersExceptSender()
        {
            // Arrange
            var room = CreateRoom();
            var sender = CreateRegularUser("sender");
            var mockUser1 = new Mock<IUser>();
            var mockUser2 = new Mock<IUser>();

            mockUser1.Setup(u => u.Username).Returns("receiver1");
            mockUser2.Setup(u => u.Username).Returns("receiver2");

            room.Register(sender);
            room.Register(mockUser1.Object);
            room.Register(mockUser2.Object);

            // Act
            room.SendMessage("sender", "Test mesajı");

            // Assert
            mockUser1.Verify(
                u => u.ReceiveMessage(It.Is<ChatMessage>(
                    m => m.SenderUsername == "sender" &&
                         m.Type == MessageType.Public)),
                Times.Once);

            mockUser2.Verify(
                u => u.ReceiveMessage(It.Is<ChatMessage>(
                    m => m.SenderUsername == "sender" &&
                         m.Type == MessageType.Public)),
                Times.Once);
        }

        [Fact]
        public void SendMessage_ShouldNotDeliverToSender()
        {
            // Arrange
            var room = CreateRoom();
            var mockSender = new Mock<IUser>();
            mockSender.Setup(u => u.Username).Returns("sender");

            room.Register(mockSender.Object);

            // Act
            room.SendMessage("sender", "Test mesajı");

            // Assert — sender kendine mesaj almamalı
            mockSender.Verify(
                u => u.ReceiveMessage(It.Is<ChatMessage>(
                    m => m.Type == MessageType.Public)),
                Times.Never);
        }

        [Fact]
        public void SendMessage_ShouldReturnCorrectDeliveredCount()
        {
            // Arrange
            var (room, admin, user1, user2) = CreatePopulatedRoom();

            // Act
            var result = room.SendMessage("user1", "Merhaba!");

            // Assert — user1 hariç 2 kişiye iletilmeli (admin + user2)
            result.DeliveredCount.Should().Be(2);
        }

        [Fact]
        public void SendMessage_WhenSenderNotInRoom_ShouldReturnFail()
        {
            // Arrange
            var room = CreateRoom();

            // Act
            var result = room.SendMessage("yok_kullanici", "Merhaba!");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("bulunamadı");
        }

        // SEND PRIVATE MESSAGE — Özel mesaj testleri

        [Fact]
        public void SendPrivateMessage_WithValidUsers_ShouldReturnSuccess()
        {
            // Arrange
            var (room, _, user1, _) = CreatePopulatedRoom();

            // Act
            var result = room.SendPrivateMessage("user1", "user2", "Özel mesaj");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.SenderUsername.Should().Be("user1");
            result.ReceiverUsername.Should().Be("user2");
            result.MessageType.Should().Be(MessageType.Private);
            result.DeliveredCount.Should().Be(1);
        }

        [Fact]
        public void SendPrivateMessage_ShouldDeliverOnlyToReceiver()
        {
            // Arrange
            var room = CreateRoom();
            var sender = CreateRegularUser("sender");
            var mockReceiver = new Mock<IUser>();
            var mockOther = new Mock<IUser>();

            mockReceiver.Setup(u => u.Username).Returns("receiver");
            mockOther.Setup(u => u.Username).Returns("other");

            room.Register(sender);
            room.Register(mockReceiver.Object);
            room.Register(mockOther.Object);

            // Act
            room.SendPrivateMessage("sender", "receiver", "Özel mesaj");

            // Assert
            mockReceiver.Verify(
                u => u.ReceiveMessage(It.Is<ChatMessage>(
                    m => m.Type == MessageType.Private &&
                         m.SenderUsername == "sender")),
                Times.Once);

            // Diğer kullanıcıya iletilmemeli
            mockOther.Verify(
                u => u.ReceiveMessage(It.Is<ChatMessage>(
                    m => m.Type == MessageType.Private)),
                Times.Never);
        }

        [Fact]
        public void SendPrivateMessage_WhenReceiverNotFound_ShouldReturnFail()
        {
            // Arrange
            var (room, _, user1, _) = CreatePopulatedRoom();

            // Act
            var result = room.SendPrivateMessage("user1", "yok_kullanici", "Mesaj");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("bulunamadı");
        }

        [Fact]
        public void SendPrivateMessage_WhenSenderNotFound_ShouldReturnFail()
        {
            // Arrange
            var (room, _, _, user2) = CreatePopulatedRoom();

            // Act
            var result = room.SendPrivateMessage("yok_kullanici", "user2", "Mesaj");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("bulunamadı");
        }

        // BROADCAST — Yetki testleri

        [Fact]
        public void Broadcast_WhenCalledByAdmin_ShouldReturnSuccess()
        {
            // Arrange
            var (room, admin, _, _) = CreatePopulatedRoom();

            // Act
            var result = room.Broadcast("admin", "Sistem mesajı!");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.MessageType.Should().Be(MessageType.Broadcast);
        }

        [Fact]
        public void Broadcast_WhenCalledByAdmin_ShouldDeliverToAllUsers()
        {
            // Arrange
            var room = CreateRoom();
            var admin = CreateAdminUser("admin");
            var mockUser1 = new Mock<IUser>();
            var mockUser2 = new Mock<IUser>();

            mockUser1.Setup(u => u.Username).Returns("user1");
            mockUser2.Setup(u => u.Username).Returns("user2");

            room.Register(admin);
            room.Register(mockUser1.Object);
            room.Register(mockUser2.Object);

            // Act
            room.Broadcast("admin", "Duyuru!");

            // Assert
            mockUser1.Verify(
                u => u.ReceiveMessage(It.Is<ChatMessage>(
                    m => m.Type == MessageType.Broadcast)),
                Times.Once);

            mockUser2.Verify(
                u => u.ReceiveMessage(It.Is<ChatMessage>(
                    m => m.Type == MessageType.Broadcast)),
                Times.Once);
        }

        [Fact]
        public void Broadcast_WhenCalledByRegularUser_ShouldReturnFail()
        {
            // Arrange
            var (room, _, user1, _) = CreatePopulatedRoom();

            // Act
            var result = room.Broadcast("user1", "Yetkisiz broadcast!");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("yetkisine sahip değil");
        }

        [Fact]
        public void Broadcast_WhenSenderNotFound_ShouldReturnFail()
        {
            // Arrange
            var room = CreateRoom();

            // Act
            var result = room.Broadcast("yok_kullanici", "Broadcast!");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("bulunamadı");
        }

        // KICK USER — Yetki ve senaryo testleri

        [Fact]
        public void KickUser_WhenCalledByAdmin_ShouldRemoveTargetFromRoom()
        {
            // Arrange
            var (room, admin, user1, _) = CreatePopulatedRoom();

            // Act
            var result = room.KickUser("admin", "user1");

            // Assert
            result.IsSuccess.Should().BeTrue();
            room.GetActiveUsers().Should().NotContain(u => u.Username == "user1");
        }

        [Fact]
        public void KickUser_WhenCalledByRegularUser_ShouldReturnFail()
        {
            // Arrange
            var (room, _, user1, user2) = CreatePopulatedRoom();

            // Act
            var result = room.KickUser("user1", "user2");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("yetkisine sahip değil");
        }

        [Fact]
        public void KickUser_WhenTargetNotFound_ShouldReturnFail()
        {
            // Arrange
            var (room, admin, _, _) = CreatePopulatedRoom();

            // Act
            var result = room.KickUser("admin", "yok_kullanici");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("bulunamadı");
        }

        [Fact]
        public void KickUser_WhenAdminKicksSelf_ShouldReturnFail()
        {
            // Arrange
            var (room, admin, _, _) = CreatePopulatedRoom();

            // Act
            var result = room.KickUser("admin", "admin");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Kendinizi");
        }

        [Fact]
        public void KickUser_ShouldNotifyRemainingUsers()
        {
            // Arrange
            var room = CreateRoom();
            var admin = CreateAdminUser("admin");
            var target = CreateRegularUser("target");
            var mockUser = new Mock<IUser>();
            mockUser.Setup(u => u.Username).Returns("observer");

            room.Register(admin);
            room.Register(target);
            room.Register(mockUser.Object);

            // Act
            room.KickUser("admin", "target");

            // Assert — kalan kullanıcıya sistem mesajı iletilmeli
            mockUser.Verify(
                u => u.ReceiveMessage(It.Is<ChatMessage>(
                    m => m.Type == MessageType.System &&
                         m.Content.Contains("target"))),
                Times.AtLeastOnce);
        }

        // COLLEAGUE — RegularUser testleri

        [Fact]
        public void RegularUser_SendMessage_ShouldDelegateToMediator()
        {
            // Arrange
            var mockMediator = new Mock<IChatMediator>();
            mockMediator
                .Setup(m => m.SendMessage("user1", "Merhaba!"))
                .Returns(ChatResult.Success("OK", "user1", 1, MessageType.Public));

            var user = CreateRegularUser("user1");
            user.SetMediator(mockMediator.Object);

            // Act
            user.SendMessage("Merhaba!");

            // Assert
            mockMediator.Verify(
                m => m.SendMessage("user1", "Merhaba!"),
                Times.Once);
        }

        [Fact]
        public void RegularUser_SendPrivateMessage_ShouldDelegateToMediator()
        {
            // Arrange
            var mockMediator = new Mock<IChatMediator>();
            mockMediator
                .Setup(m => m.SendPrivateMessage("user1", "user2", "Özel"))
                .Returns(ChatResult.Success("OK", "user1", 1,
                    MessageType.Private, "user2"));

            var user = CreateRegularUser("user1");
            user.SetMediator(mockMediator.Object);

            // Act
            user.SendPrivateMessage("user2", "Özel");

            // Assert
            mockMediator.Verify(
                m => m.SendPrivateMessage("user1", "user2", "Özel"),
                Times.Once);
        }

        [Fact]
        public void RegularUser_LeaveRoom_ShouldCallUnregister()
        {
            // Arrange
            var mockMediator = new Mock<IChatMediator>();
            var user = CreateRegularUser("user1");
            user.SetMediator(mockMediator.Object);

            // Act
            user.LeaveRoom();

            // Assert
            mockMediator.Verify(
                m => m.Unregister(user),
                Times.Once);
        }

        [Fact]
        public void RegularUser_ReceiveMessage_ShouldAddToHistory()
        {
            // Arrange
            var user = CreateRegularUser("user1");
            var message = ChatMessage.Public("sender", "Test mesajı");

            // Act
            user.ReceiveMessage(message);

            // Assert
            user.GetMessageHistory().Should().ContainSingle();
            user.GetMessageHistory()[0].Content.Should().Be("Test mesajı");
        }

        [Fact]
        public void RegularUser_SendMessage_WhenNoMediator_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var user = CreateRegularUser("user1");

            // Act
            var act = () => user.SendMessage("Merhaba!");

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*odaya katılmadı*");
        }

        // COLLEAGUE — AdminUser testleri

        [Fact]
        public void AdminUser_Broadcast_ShouldDelegateToMediator()
        {
            // Arrange
            var mockMediator = new Mock<IChatMediator>();
            mockMediator
                .Setup(m => m.Broadcast("admin", "Duyuru!"))
                .Returns(ChatResult.Success("OK", "admin", 3, MessageType.Broadcast));

            var admin = CreateAdminUser("admin");
            admin.SetMediator(mockMediator.Object);

            // Act
            admin.Broadcast("Duyuru!");

            // Assert
            mockMediator.Verify(
                m => m.Broadcast("admin", "Duyuru!"),
                Times.Once);
        }

        [Fact]
        public void AdminUser_KickUser_ShouldDelegateToMediator()
        {
            // Arrange
            var mockMediator = new Mock<IChatMediator>();
            mockMediator
                .Setup(m => m.KickUser("admin", "target"))
                .Returns(ChatResult.Success("OK", "admin", 1,
                    MessageType.System, "target"));

            var admin = CreateAdminUser("admin");
            admin.SetMediator(mockMediator.Object);

            // Act
            admin.KickUser("target");

            // Assert
            mockMediator.Verify(
                m => m.KickUser("admin", "target"),
                Times.Once);
        }

        [Fact]
        public void AdminUser_LeaveRoom_ShouldCallUnregister()
        {
            // Arrange
            var mockMediator = new Mock<IChatMediator>();
            var admin = CreateAdminUser("admin");
            admin.SetMediator(mockMediator.Object);

            // Act
            admin.LeaveRoom();

            // Assert
            mockMediator.Verify(
                m => m.Unregister(admin),
                Times.Once);
        }

        // COLLEAGUE — BotUser testleri

        [Fact]
        public void BotUser_WhenReceivesPublicMessage_ShouldSendAutoReply()
        {
            // Arrange
            var mockMediator = new Mock<IChatMediator>();
            mockMediator
                .Setup(m => m.SendPrivateMessage(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(ChatResult.Success("OK", "bot", 1, MessageType.Private));

            var bot = CreateBotUser("bot");
            bot.SetMediator(mockMediator.Object);

            var message = ChatMessage.Public("user1", "Merhaba bot!");

            // Act
            bot.ReceiveMessage(message);

            // Assert — bot user1'e otomatik yanıt vermeli
            mockMediator.Verify(
                m => m.SendPrivateMessage(
                    "bot",
                    "user1",
                    It.Is<string>(s => s.Contains("user1"))),
                Times.Once);
        }

        [Fact]
        public void BotUser_WhenReceivesPrivateMessage_ShouldNotAutoReply()
        {
            // Arrange
            var mockMediator = new Mock<IChatMediator>();
            var bot = CreateBotUser("bot");
            bot.SetMediator(mockMediator.Object);

            var privateMessage = ChatMessage.Private("user1", "bot", "Özel mesaj");

            // Act
            bot.ReceiveMessage(privateMessage);

            // Assert — özel mesaja otomatik yanıt vermemeli
            mockMediator.Verify(
                m => m.SendPrivateMessage(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void BotUser_WhenReceivesOwnMessage_ShouldNotAutoReply()
        {
            // Arrange
            var mockMediator = new Mock<IChatMediator>();
            var bot = CreateBotUser("bot");
            bot.SetMediator(mockMediator.Object);

            // Bot kendi mesajını alıyor
            var ownMessage = ChatMessage.Public("bot", "Ben botum");

            // Act
            bot.ReceiveMessage(ownMessage);

            // Assert — kendi mesajına yanıt vermemeli
            mockMediator.Verify(
                m => m.SendPrivateMessage(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        // GUARD CLAUSE TESTLERİ

        [Fact]
        public void SendMessage_WhenContentIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var room = CreateRoom();

            // Act
            var act = () => room.SendMessage("user1", string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("content");
        }

        [Fact]
        public void SendMessage_WhenSenderUsernameIsWhitespace_ShouldThrowArgumentException()
        {
            // Arrange
            var room = CreateRoom();

            // Act
            var act = () => room.SendMessage("   ", "Mesaj");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("senderUsername");
        }

        [Fact]
        public void SendPrivateMessage_WhenContentIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var room = CreateRoom();

            // Act
            var act = () => room.SendPrivateMessage("user1", "user2", string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("content");
        }

        [Fact]
        public void Broadcast_WhenContentIsWhitespace_ShouldThrowArgumentException()
        {
            // Arrange
            var room = CreateRoom();

            // Act
            var act = () => room.Broadcast("admin", "   ");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("content");
        }

        [Fact]
        public void KickUser_WhenTargetUsernameIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var room = CreateRoom();

            // Act
            var act = () => room.KickUser("admin", string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("targetUsername");
        }

        [Fact]
        public void Register_WhenUserIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var room = CreateRoom();

            // Act
            var act = () => room.Register(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("user");
        }

        [Fact]
        public void Unregister_WhenUserIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var room = CreateRoom();

            // Act
            var act = () => room.Unregister(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("user");
        }

        // CONSTRUCTOR NULL GUARD TESTLERİ

        [Fact]
        public void ChatRoom_WhenRoomNameIsEmpty_ShouldThrowArgumentException()
        {
            // Act
            var act = () => new ChatRoom(string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("roomName");
        }

        [Fact]
        public void ChatRoom_WhenRoomNameIsWhitespace_ShouldThrowArgumentException()
        {
            // Act
            var act = () => new ChatRoom("   ");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("roomName");
        }

        [Fact]
        public void RegularUser_WhenUsernameIsEmpty_ShouldThrowArgumentException()
        {
            // Act
            var act = () => new RegularUser(string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("username");
        }

        [Fact]
        public void AdminUser_WhenUsernameIsWhitespace_ShouldThrowArgumentException()
        {
            // Act
            var act = () => new AdminUser("   ");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("username");
        }

        [Fact]
        public void BotUser_WhenUsernameIsEmpty_ShouldThrowArgumentException()
        {
            // Act
            var act = () => new BotUser(string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("username");
        }

        [Fact]
        public void RegularUser_SetMediator_WhenMediatorIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var user = CreateRegularUser();

            // Act
            var act = () => user.SetMediator(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("mediator");
        }

        [Fact]
        public void AdminUser_SetMediator_WhenMediatorIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var admin = CreateAdminUser();

            // Act
            var act = () => admin.SetMediator(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("mediator");
        }

        [Fact]
        public void BotUser_SetMediator_WhenMediatorIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var bot = CreateBotUser();

            // Act
            var act = () => bot.SetMediator(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("mediator");
        }
    }
}