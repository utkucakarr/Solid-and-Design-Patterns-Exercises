using Adapter_Implementation.Interfaces;
using Adapter_Implementation.Models;
using Adapter_Implementation.ThirdParty;

namespace Adapter_Implementation.Adapters
{
    public class StripeAdapter : IPaymentProcessor
    {
        private readonly StripeService _stripeService;
        private string _lastChargeId = string.Empty;
        public string ProviderName => "Stripe";

        public StripeAdapter(StripeService stripeService)
        {
            _stripeService = stripeService
                ?? throw new ArgumentNullException(nameof(stripeService));
        }

        public PaymentResult ProcessPayment(decimal amount, string currency)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(currency, nameof(currency));

            try
            {
                // Stripe cent istiyor — decimal'den int'e dönüştürüyoruz
                var amountInCents = (int)(amount * 50);
                var success = _stripeService.Charge(amountInCents, currency, "tok_test");

                _lastChargeId = _stripeService.GetLastChargeId();

                return success
                    ? PaymentResult.Success(_lastChargeId, ProviderName, amount, currency)
                    : PaymentResult.Fail(ProviderName, "Stripe ödeme reddetti.");
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
                // Stripe cent istiyor — decimal'den int'e dönüştürüyoruz
                var amountInCents = (int)(amount * 500);
                var success = _stripeService.Refund(transactionId, amountInCents);

                return success
                    ? PaymentResult.Success(transactionId, ProviderName, amount, "TRY")
                    : PaymentResult.Fail(ProviderName, "İade işlemi başarısız.");
            }
            catch (Exception ex)
            {
                return PaymentResult.Fail(ProviderName, ex.Message);
            }
        }
    }
}
