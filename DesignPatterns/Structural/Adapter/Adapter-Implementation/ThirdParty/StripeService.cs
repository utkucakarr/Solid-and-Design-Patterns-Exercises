namespace Adapter_Implementation.ThirdParty
{
    // Stripe'ın kendi API'si — cent cinsinden çalışıyor
    public class StripeService
    {
        public bool Charge(int amountInCents, string currency, string token)
        {
            Console.WriteLine($"[Stripe SDK] Charge({amountInCents} cents, " +
                              $"{currency}, {token})");
            return true;
        }

        public bool Refund(string chargeId, int amountInCents)
        {
            Console.WriteLine($"[Stripe SDK] Refund({chargeId}, {amountInCents} cents)");
            return true;
        }

        // Stripe transaction ID üretir
        public string GetLastChargeId() => $"ch_{Guid.NewGuid():N}";
    }
}
