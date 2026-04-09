using Facade_Violation.Subsystems;

namespace Facade_Violation
{
    // Client tüm alt sistemleri biliyor ve adım sırasını kendisi yönetiyor!
    public class BankingClientBad
    {
        private readonly BalanceServiceBad _balance = new();
        private readonly FraudServiceBad _fraud = new();
        private readonly TransferServiceBad _transfer = new();
        private readonly NotificationServiceBad _notification = new();
        private readonly AuditServiceBad _audit = new();

        public void Transfer(string fromAccount, string toAccount, decimal amount)
        {
            // Orchestration mantığı client'ta - Facade ihlali!

            // 1. Bakiye kontrolü
            var balance = _balance.GetBalance(fromAccount);
            if (balance < amount)
            {
                Console.WriteLine("Yetersiz bakiye!");
                return;
            }

            // 2. Fraud kontrolü
            if(_fraud.IsSuspicious(fromAccount, amount))
            {
                Console.WriteLine("Şüpheli işlem tespit edildi, transfer iptal!");
                return;
            }

            // 3. Bakiye düşür
            _balance.DeductBalance(fromAccount, amount);

            // 4. Transferi gerçekleştir
            var referenceId = _transfer.ExecuteTransfer(fromAccount, toAccount, amount);

            // 5. Alıcı bakiyesini güncelle
            _balance.AddBalance(toAccount, amount);

            // 6. Bildirimler gönder
            _notification.NotifySender(fromAccount, amount, referenceId);
            _notification.NotifyReceiver(toAccount, amount, referenceId);

            // 7. Audit log
            _audit.LogTransfer(fromAccount, toAccount, amount, referenceId);

            Console.WriteLine($"Transfer tamamlandı. Ref: {referenceId}");

            // Yeni adım (limit kontrolü, döviz kuru) = her client'a dokun!
            // Rollback mantığı client'ta yok — bakiye düştü ama transfer başarısız olursa?
            // Her yeni banking client aynı 7 adımı tekrar yazmak zorunda
        }
    }
}
