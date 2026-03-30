namespace Singleton_Violation.Logging
{
    public class PaymentService
    {    
        // Üçüncü Logger instance'ı!
        private readonly Logger _logger = new("app.log");

        public void ProcessPayment(decimal amount)
            => _logger.Log($"[PaymentService] Ödeme işlendi: {amount} TL");
    }
}
