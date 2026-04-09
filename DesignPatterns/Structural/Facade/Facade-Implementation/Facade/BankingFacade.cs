using Facade_Implementation.Interfaces;
using Facade_Implementation.Models;

namespace Facade_Implementation.Facade
{
    // Facade - karmaşık transfer sürecini tek bir basit API'ye indirgiyor
    public class BankingFacade
    {
        private readonly IBalanceService _balanceService;
        private readonly IFraudService _fraudService;
        private readonly ITransferService _transferService;
        private readonly INotificationService _notificationService;
        private readonly IAuditService _auditService;

        public BankingFacade(
            IBalanceService balanceService,
            IFraudService fraudService,
            ITransferService transferService,
            INotificationService notificationService,
            IAuditService auditService)
        {
            _balanceService = balanceService
                ?? throw new ArgumentNullException(nameof(balanceService));
            _fraudService = fraudService
                ?? throw new ArgumentNullException(nameof(fraudService));
            _transferService = transferService
                ?? throw new ArgumentNullException(nameof(transferService));
            _notificationService = notificationService
                ?? throw new ArgumentNullException(nameof(notificationService));
            _auditService = auditService
                ?? throw new ArgumentNullException(nameof(auditService));
        }

        // Client sadece bu metodu çağırıyor - alt sistem detayları tamamen gizli
        public TransferResult Transfer(string fromAccount, string toAccount, decimal amount)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fromAccount);
            ArgumentException.ThrowIfNullOrWhiteSpace(toAccount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

            try
            {
                // Bakiye kontrolü
                var balance = _balanceService.GetBalance(fromAccount);
                if (balance < amount)
                {
                    return TransferResult.Fail("Yetersiz bakiye.");
                }

                // 2. Fraud kontrolü
                if (_fraudService.IsSuspicious(fromAccount, amount))
                    return TransferResult.Fail("Şüpheli işlem tespit edildi.");

                // 3. Bakiye düşür
                _balanceService.DeductBalance(fromAccount, amount);

                // 4. Transferi gerçekleştir
                var referenceId = _transferService.ExecuteTransfer(fromAccount, toAccount, amount);

                // 5. Alıcı bakiyesini güncelle
                _balanceService.AddBalance(toAccount, amount);

                // 6. Bildirimler
                _notificationService.NotifySender(fromAccount, amount, referenceId);
                _notificationService.NotifyReceiver(fromAccount, amount, referenceId);

                // 7. Audit log
                _auditService.LogTransfer(fromAccount, toAccount, amount, referenceId);

                return TransferResult.Success(referenceId, fromAccount, toAccount, amount);
            }
            catch (Exception ex)
            {
                return TransferResult.Fail(ex.Message);
            }
        }
    }
}