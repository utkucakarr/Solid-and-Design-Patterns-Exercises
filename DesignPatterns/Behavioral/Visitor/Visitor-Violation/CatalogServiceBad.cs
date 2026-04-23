namespace Visitor_Violation
{
    // Ürün tipleri string/enum ile ayrıştırılıyor — type checking anti-pattern
    // Yeni bir ürün tipi eklenince tüm if-else blokları güncellenmeli
    // Yeni bir operasyon (ör: kargo maliyeti) eklenince bu dev sınıf tekrar açılmalı
    // Open/Closed Principle ihlali: her değişiklik bu sınıfı patlatır
    // Her operasyon (vergi, indirim, rapor) aynı sınıfta — Single Responsibility ihlali
    public class CatalogServiceBad
    {
        // Ürün tipi string ile taşınıyor — tip güvenliği yok
        public decimal CalculateTax(string productType, decimal basePrice, decimal weight = 0)
        {
            // if-else zinciri — her yeni tip buraya eklenmeli
            if (productType == "Physical")
            {
                // Fiziksel ürün: %18 KDV + ağırlık bazlı ek vergi
                var weightTax = weight * 0.5m;
                return basePrice * 0.18m + weightTax;
            }
            else if (productType == "Digital")
            {
                // Dijital ürün: %8 KDV — ama bu kural değişirse tüm metot açılır
                return basePrice * 0.08m;
            }
            else if (productType == "Subscription")
            {
                // Abonelik: %18 KDV — aylık bazda hesaplanmalı ama burada düz
                return basePrice * 0.18m;
            }
            else
            {
                // Bilinmeyen tip — runtime'a kadar fark edilmez
                throw new ArgumentException($"Bilinmeyen ürün tipi: {productType}");
            }
        }

        // İndirim hesabı da aynı sınıfta — tamamen farklı bir sorumluluk
        public decimal CalculateDiscount(string productType, decimal basePrice, bool isPremiumCustomer)
        {
            // Aynı if-else zinciri tekrar — kod tekrarı
            if (productType == "Physical")
            {
                return isPremiumCustomer ? basePrice * 0.15m : basePrice * 0.05m;
            }
            else if (productType == "Digital")
            {
                return isPremiumCustomer ? basePrice * 0.20m : basePrice * 0.10m;
            }
            else if (productType == "Subscription")
            {
                // Abonelikte indirim mantığı karmaşık ama hepsi burada
                return isPremiumCustomer ? basePrice * 0.25m : 0m;
            }
            else
            {
                throw new ArgumentException($"Bilinmeyen ürün tipi: {productType}");
            }
        }

        // Rapor üretimi de aynı sınıfta — üçüncü farklı sorumluluk
        public string GenerateReport(string productType, string productName, decimal basePrice)
        {
            // Yine aynı if-else — "GiftProduct" eklenince 3 metot birden güncellenmeli
            if (productType == "Physical")
            {
                return $"[FİZİKSEL] {productName} | Fiyat: {basePrice:C} | Kargo: Gerekli";
            }
            else if (productType == "Digital")
            {
                return $"[DİJİTAL] {productName} | Fiyat: {basePrice:C} | Anında Teslimat";
            }
            else if (productType == "Subscription")
            {
                return $"[ABONELİK] {productName} | Aylık: {basePrice:C} | Otomatik Yenileme";
            }
            else
            {
                throw new ArgumentException($"Bilinmeyen ürün tipi: {productType}");
            }
        }
    }
}