namespace Adapter_Violation.Payment
{
    // Client her servisin API'sini doğrudan biliyor
    // Yeni sağlayıcı eklenince tüm client kodu değişiyor.
    public class CheckoutServiceBad
    {
        private readonly PayPallServiceBad _payPallServiceBad = new();
        private readonly IyzicoServiceBad _iyzicoServiceBad = new();
        private readonly StripeServiceBad _stripeServiceBad = new();

        public void ProcessPayment(string provider, decimal amount)
        {
            // Her sağlayıcı için ayrı kod - OCP ihlali!
            if (provider == "paypall")
            {
                var result = _payPallServiceBad.MakePayment((double)amount, "TRY");
                Console.WriteLine($"PayPal transaction: {result}");
            }
            else if (provider == "stripe")
            {
                var cents = (int)(amount * 50);
                var result = _stripeServiceBad.Charge(cents, "try", "tok_test");
                Console.WriteLine($"Stripe success {result}");
            }
            else if (provider == "iyzico")
            {
                var result = _iyzicoServiceBad.OdemeYap(amount, "TRY");
                Console.WriteLine($"İyzico status: {result}");
            }
            // Yeni sağlayıcı = buraya yeni else if ekle!
        }
    }
}
