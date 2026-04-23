namespace ChainOfResponsibility_Violation
{
    // Tüm kontrol adımları tek bir sınıfa yığılmış
    // Yeni bir kontrol eklemek için bu dev metodu düzenlemek gerekiyor
    // Her adım birbirine sıkı sıkıya bağlı (tight coupling)
    // Her adımı bağımsız test etmek mümkün değil
    // Open/Closed Principle ihlali: her değişiklik sınıfı patlatır
    public class CheckoutServiceBad
    {
        public string ProcessOrder(string productId, int quantity, string customerId, decimal accountBalance)
        {
            // Guard clause yok — null/boş değerler patlatır

            // ADIM 1: Stok kontrolü — ayrı bir handler olmalıydı
            var stock = GetStock(productId);
            if (stock < quantity)
            {
                return $"Stok yetersiz. Mevcut: {stock}, İstenen: {quantity}";
            }

            // ADIM 2: Fraud kontrolü — ayrı bir handler olmalıydı
            var fraudScore = GetFraudScore(customerId);
            if (fraudScore > 75)
            {
                return $"Şüpheli işlem tespit edildi. Fraud skoru: {fraudScore}";
            }

            // ADIM 3: Ödeme kontrolü — ayrı bir handler olmalıydı
            var totalPrice = GetPrice(productId) * quantity;
            if (accountBalance < totalPrice)
            {
                return $"Bakiye yetersiz. Gereken: {totalPrice:C}, Mevcut: {accountBalance:C}";
            }

            // ADIM 4: Kargo kontrolü — ayrı bir handler olmalıydı
            var shippingAvailable = CheckShipping(productId);
            if (!shippingAvailable)
            {
                return "Bu ürün için kargo hizmeti mevcut değil";
            }

            // ADIM 5: Hepsini bir arada yap — rollback mekanizması yok
            DeductStock(productId, quantity);
            ChargeAccount(customerId, totalPrice);
            var trackingCode = CreateShipment(productId, customerId);

            return $"Sipariş onaylandı! Takip kodu: {trackingCode}";
        }

        // Tüm yardımcı metodlar aynı sınıfta — Single Responsibility ihlali
        private int GetStock(string productId) => productId == "PROD-001" ? 50 : 0;
        private decimal GetFraudScore(string customerId) => customerId == "FRAUD-USER" ? 90m : 20m;
        private decimal GetPrice(string productId) => 299.99m;
        private bool CheckShipping(string productId) => productId != "DIGITAL-ONLY";
        private void DeductStock(string productId, int quantity) { }
        private void ChargeAccount(string customerId, decimal amount) { }
        private string CreateShipment(string productId, string customerId) => $"TRK-{Guid.NewGuid().ToString()[..8].ToUpper()}";

    }
}
