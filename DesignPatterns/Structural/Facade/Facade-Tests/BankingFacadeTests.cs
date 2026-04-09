using Facade_Implementation.Facade;
using Facade_Implementation.Interfaces;
using FluentAssertions;
using Moq;

namespace Facade_Tests
{
    public class BankingFacadeTests
    {
        private readonly Mock<IBalanceService> _balanceMock;
        private readonly Mock<IFraudService> _fraudMock;
        private readonly Mock<ITransferService> _transferMock;
        private readonly Mock<INotificationService> _notificationMock;
        private readonly Mock<IAuditService> _auditMock;
        private readonly BankingFacade _sut;

        public BankingFacadeTests()
        {
            _balanceMock = new Mock<IBalanceService>();
            _fraudMock = new Mock<IFraudService>();
            _transferMock = new Mock<ITransferService>();
            _notificationMock = new Mock<INotificationService>();
            _auditMock = new Mock<IAuditService>();

            // Happy path kurulumu
            _balanceMock
                .Setup(b => b.GetBalance(It.IsAny<string>()))
                .Returns(10_000m);
            _fraudMock
                .Setup(f => f.IsSuspicious(It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns(false);
            _transferMock
                .Setup(t => t.ExecuteTransfer(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
                .Returns("TRF_TEST");

            _sut = new BankingFacade(
                _balanceMock.Object,
                _fraudMock.Object,
                _transferMock.Object,
                _notificationMock.Object,
                _auditMock.Object
                );
        }

        // --- Transfer - Baþarýlý Senaryosu ---
        [Fact]
        public void Transfer_WithValidInputs_ShouldReturnSuccess()
        {
            var result = _sut.Transfer("ACC_001", "ACC_002", 1000);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Transfer_ShouldReturenReferenceId()
        {
            var result = _sut.Transfer("ACC_001", "ACC_002", 1000);

            result.ReferenceId.Should().Be("TRF_TEST");
        }

        [Fact]
        public void Transfer_ShouldReturnCorrenctAmount()
        {
            var result = _sut.Transfer("ACC_001", "ACC_002", 1000);

            result.Amount.Should().Be(1000);
        }

        // --- Alt Sistem Çaðrý Doðrulama ---
        [Fact]
        public void Transfer_ShouldCheckBalanceBeforeTransfer()
        {
            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _balanceMock.Verify(b => b.GetBalance("ACC_001"), Times.Once);
        }

        [Fact]
        public void Transfer_ShouldCheckFraud()
        {
            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _fraudMock.Verify(f => f.IsSuspicious("ACC_001", 1000), Times.Once);
        }

        [Fact]
        public void Transfer_ShouldDeductFromOrder()
        {
            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _balanceMock.Verify(b => b.DeductBalance("ACC_001", 1000), Times.Once);
        }

        [Fact]
        public void Tansfer_ShouldAddToReceiver()
        {
            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _balanceMock.Verify(b => b.AddBalance("ACC_002", 1000), Times.Once);
        }

        [Fact]
        public void Transfer_ShouldNotifySender()
        {
            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _notificationMock.Verify(n => n.NotifySender("ACC_001", 1000, "TRF_TEST"),
                Times.Once);
        }

        [Fact]
        public void Transfer_ShouldNotifyReciever()
        {
            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _notificationMock.Verify(n => n.NotifyReceiver("ACC_001", 1000, "TRF_TEST"),
                Times.Once);
        }

        [Fact]
        public void Transfer_ShouldLogAudit()
        {
            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _auditMock.Verify(a => a.LogTransfer("ACC_001", "ACC_002", 1000, "TRF_TEST"),
                Times.Once);
        }

        // --- Yetersiz Bakiye Senaryosu ---

        [Fact]
        public void Transfer_WhenInsufficientBalance_ShouldReturnFail()
        {
            _balanceMock.Setup(b => b.GetBalance(It.IsAny<string>())).Returns(500m);

            var result = _sut.Transfer("ACC_001", "ACC_002", 1000);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void Transfer_WhenInsufficientBalance_ShouldNotExecuteTransfer()
        {
            _balanceMock.Setup(b => b.GetBalance(It.IsAny<string>())).Returns(500m);

            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _transferMock.Verify(t => t.ExecuteTransfer(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        }

        [Fact]
        public void Transfer_WhenInsufficientBalance_ShouldNotSendNotification()
        {
            _balanceMock.Setup(b => b.GetBalance(It.IsAny<string>())).Returns(500m);

            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _notificationMock.Verify(
                n => n.NotifySender(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()),
                Times.Never);
        }

        // --- Fraud Senaryosu ---
        [Fact]
        public void Transfer_WhenFraudDetected_ShouldReturnFail()
        {
            _fraudMock.Setup(f => f.IsSuspicious(It.IsAny<string>(), It.IsAny<decimal>()))
                      .Returns(true);

            var result = _sut.Transfer("ACC_001", "ACC_002", 1000);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void Transfer_WhenFraudDetected_ShouldNotDeductBalance()
        {
            _fraudMock.Setup(f => f.IsSuspicious(It.IsAny<string>(), It.IsAny<decimal>()))
                      .Returns(true);

            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _balanceMock.Verify(
                b => b.DeductBalance(It.IsAny<string>(), It.IsAny<decimal>()),
                Times.Never);
        }

        [Fact]
        public void Transfer_WhenFraudDetected_ShouldNotLogAudit()
        {
            _fraudMock.Setup(f => f.IsSuspicious(It.IsAny<string>(), It.IsAny<decimal>()))
                      .Returns(true);

            _sut.Transfer("ACC_001", "ACC_002", 1000);

            _auditMock.Verify(
                a => a.LogTransfer(It.IsAny<string>(), It.IsAny<string>(),
                                   It.IsAny<decimal>(), It.IsAny<string>()),
                Times.Never);
        }

        // --- Guard Clause Testleri ---
        [Fact]
        public void Transfer_WithEmptyFromAccount_ShouldThrowArgumentException()
        {
            var act = () => _sut.Transfer(" ", "ACC_002", 1000);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Transfer_WithEmptyToAccount_ShouldThrowArgumentException()
        {
            var act = () => _sut.Transfer("ACC_001", " ", 1000);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Transfer_WithZeroAmount_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _sut.Transfer("ACC_001", "ACC_002", 0);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Transfer_WithNegativeAmount_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _sut.Transfer("ACC_001", "ACC_002", -100);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        // --- Constructor Testleri ---

        [Fact]
        public void Constructor_WithNullBalanceService_ShouldThrowArgumentNullException()
        {
            var act = () => new BankingFacade(null!, _fraudMock.Object, _transferMock.Object,
                                              _notificationMock.Object, _auditMock.Object);

            act.Should().Throw<ArgumentNullException>().WithParameterName("balanceService");
        }

        [Fact]
        public void Constructor_WithNullFraudService_ShouldThrowArgumentNullException()
        {
            var act = () => new BankingFacade(_balanceMock.Object, null!, _transferMock.Object,
                                              _notificationMock.Object, _auditMock.Object);

            act.Should().Throw<ArgumentNullException>().WithParameterName("fraudService");
        }

        [Fact]
        public void Constructor_WithNullAuditService_ShouldThrowArgumentNullException()
        {
            var act = () => new BankingFacade(_balanceMock.Object, _fraudMock.Object, _transferMock.Object,
                                              _notificationMock.Object, null!);

            act.Should().Throw<ArgumentNullException>().WithParameterName("auditService");
        }
    }
}