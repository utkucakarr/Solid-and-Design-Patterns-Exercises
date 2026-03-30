namespace Singleton_Violation.Logging
{
    public class OrderService
    {
        // Ayrı Logger instance'ı
        private readonly Logger _logger = new("app.log");

        public void CreateOrder(string product)
            => _logger.Log($"[OrderService] Sipariş oluşturuldu: {product}");
    }
}
