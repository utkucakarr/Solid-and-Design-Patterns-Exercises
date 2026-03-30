using Singleton_Implementation.Interfaces;

namespace Singleton_Implementation.Services
{
    public class PaymentService
    {
        private readonly IAppLogger _logger;

        public PaymentService(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void ProcessPayment(decimal amount, string method)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));
            ArgumentException.ThrowIfNullOrWhiteSpace(method, nameof(method));

            _logger.Info($"[PaymentService] Ödeme işlendi -> {amount} TL | Yöntem: {method}");
        }

        public void PaymentFailed(decimal amount, string reason)
        {
            _logger.Error($"[PaymentService] Ödeme başarısız -> {amount} TL | Sebep: {reason}");
        }
    }
}
