namespace Adapter_Implementation.ThirdParty
{
    // Üçüncü parti kütüphane — değiştirilemez, olduğu gibi kullanılıyor
    // Gerçekte bu NuGet paketi olarak gelir
    public class PayPalService
    {
        // PayPal'ın kendi API'si — bizim interface'imizden farklı!
        public string MakePayment(double amount, string currency)
        {
            Console.WriteLine($"[PayPal SDK] MakePayment({amount}, {currency})");
            return $"PAYPAL_{Guid.NewGuid():N}";
        }

        public bool RefoundTransaction(string transactionId)
        {
            Console.WriteLine($"[PayPal SDK] RefundTransaction({transactionId})");
            return true;
        }
    }
}
