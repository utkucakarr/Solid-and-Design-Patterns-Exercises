using Adapter_Implementation.Interfaces;
using Adapter_Implementation.Models;
using Adapter_Implementation.ThirdParty;

namespace Adapter_Implementation.Adapters
{
    public class IyzicoAdapter : IPaymentProcessor
    {
        private readonly IyzicoService _iyzicoService;

        public string ProviderName => "Iyzico";

        public IyzicoAdapter(IyzicoService iyzicoService)
        {
            _iyzicoService = iyzicoService
                ?? throw new ArgumentNullException(nameof(iyzicoService));
        }

        public PaymentResult ProcessPayment(decimal amount, string currency)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));
            ArgumentException.ThrowIfNullOrWhiteSpace(currency, nameof(currency));

            try
            {
                // Iyzico int HTTP kodu döndürüyor — bool'a dönüştürüyoruz
                var statusCode = _iyzicoService.OdemeYap(amount, currency);
                var transactionId = _iyzicoService.ReferansKoduOlustur();

                return statusCode == 200
                    ? PaymentResult.Success(transactionId, ProviderName, amount, currency)
                    : PaymentResult.Fail(ProviderName, $"HTTP {statusCode} hatası.");
            }
            catch (Exception ex)
            {
                return PaymentResult.Fail(ProviderName, ex.Message);
            }
        }

        public PaymentResult Refund(string transactionId, decimal amount)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(transactionId, nameof(transactionId));

            try
            {
                var statusCode = _iyzicoService.IadeYap(transactionId, amount);

                return statusCode == 200
                    ? PaymentResult.Success(transactionId, ProviderName, amount, "TRY")
                    : PaymentResult.Fail(ProviderName, $"İade HTTP {statusCode} hatası.");
            }
            catch (Exception ex)
            {
                return PaymentResult.Fail(ProviderName, ex.Message);
            }
        }
    }
}
