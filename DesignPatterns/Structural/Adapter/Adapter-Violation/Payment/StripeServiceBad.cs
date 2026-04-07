namespace Adapter_Violation.Payment
{
    public class StripeServiceBad
    {
        public bool Charge(int amountInCents, string currency, string token)
        {
            Console.WriteLine($"[Stripe] {amountInCents} cent {currency} " +
                  $"token: {token} ile ödendi.");
            return true;
        }
    }
}
