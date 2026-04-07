using Adapter_Implementation.Interfaces;
using Adapter_Implementation.Models;
using Adapter_Implementation.ThirdParty;

namespace Adapter_Implementation.Adapters
{
    // Adapter — PayPal'ın API'sini IPaymentProcessor'a dönüştürüyor
    public class PaypalAdapter : IPaymentProcessor
    {
        private readonly PayPalService _payPalService;

        public string ProviderName => "PayPal";

        public PaypalAdapter(PayPalService payPalService)
        {
            _payPalService = payPalService ?? throw new ArgumentNullException(nameof(payPalService));
        }

        public PaymentResult ProcessPayment(decimal amount, string currency)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));
            ArgumentException.ThrowIfNullOrWhiteSpace(currency, nameof(currency));

            try
            {
                var transactionId = _payPalService.MakePayment((double)amount, currency);
                return PaymentResult.Success(transactionId, ProviderName, amount, currency);
            }
            catch (Exception exception)
            {
                return PaymentResult.Fail(ProviderName, exception.Message);
            }
        }

        public PaymentResult Refund(string transactionId, decimal amount)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(transactionId, nameof(transactionId));

            try{
                var success = _payPalService.RefoundTransaction(transactionId);
                return success
                    ? PaymentResult.Success(transactionId, ProviderName, amount, "TRY")
                    : PaymentResult.Fail(ProviderName, "İade işlemi başarısız.");
            }
            catch (Exception exception)
            {
                return PaymentResult.Fail(transactionId, exception.Message);
            }

        }
    }
}
