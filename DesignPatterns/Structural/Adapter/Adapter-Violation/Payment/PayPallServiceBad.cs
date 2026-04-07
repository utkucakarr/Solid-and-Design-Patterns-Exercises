namespace Adapter_Violation.Payment
{
    // Üçüncü parti servisler - farklı imzalar, değiştirilemez
    public class PayPallServiceBad
    {
        public string MakePayment(double amount, string currency)
        {
            Console.WriteLine($"[Paypal] {amount} {currency} ödendi");
            return $"PAYPAL_{Guid.NewGuid():N}";
        }
    }
}
