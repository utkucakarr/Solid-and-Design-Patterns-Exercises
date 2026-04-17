using Strategy_Violation.Models;

namespace Strategy_Violation
{
    public class ShippingServiceBad
    {
        // Tüm fiyatlandırma algoritmaları tek sınıfta toplanmış
        // Yeni kargo tipi eklemek için bu sınıf her seferinde değiştirilmek zorunda (OCP ihlali)

        public ShippingResult CalculateShipping(ShippingOrder order)
        {
            // Uzayan if/else zinciri — strateji seçimi ve hesaplama iç içe geçmiş
            if (order.ShippingType == "standard")
            {
                // sabit ücret mantığı doğrudan burada gömülü
                decimal baseFee = 15.00m;
                decimal weightFee = (decimal)order.WeightKg * 2.50m;
                decimal totalCost = baseFee * weightFee;

                return new ShippingResult()
                {
                    IsSuccess = true,
                    Message = "Standart kargo hesaplandı.",
                    Cost = totalCost,
                    CarrierName = "MNG Kargo",
                    EstimatedDays = 3
                };
            }
            else if (order.ShippingType == "express")
            {
                // Express algoritması da aynı metot içinde - tek sorumluluk yok
                decimal baseFee = 35.00m;
                decimal weightFee = (decimal)order.WeightKg * 5.00m;
                decimal totalCost = baseFee + weightFee;

                return new ShippingResult()
                {
                    IsSuccess = true,
                    Message = "Hızlı kargo hesaplanda",
                    Cost = totalCost,
                    CarrierName = "Yurtiçi Kargo",
                    EstimatedDays = 1
                };
            }
            else if (order.ShippingType == "free")
            {
                // Ücretsiz kargo eşiği hardcoded — konfigürasyon yok
                if(order.OrderTotal < 500.00m)
                {
                    return new ShippingResult()
                    {
                        IsSuccess = false,
                        Message = "Ücretsiz kargo için minimum 500 TL sipariş gereklidir.",
                        Cost = 0,
                        CarrierName = string.Empty,
                        EstimatedDays = 0
                    };
                }

                return new ShippingResult
                {
                    IsSuccess = true,
                    Message = "Ücretsiz kargo uygulandı.",
                    Cost = 0,
                    CarrierName = "Aras Kargo",
                    EstimatedDays = 5
                };
            }
            else if (order.ShippingType == "member")
            {
                // Üyelik tipi kontrolü de bu metodun içinde — sorumluluklar ayrılmamış
                if(order.MemberShipType != "premium")
                {
                    return new ShippingResult
                    {
                        IsSuccess = false,
                        Message = "Üye kargosu sadece Premium üyelere özeldir.",
                        Cost = 0,
                        CarrierName = string.Empty,
                        EstimatedDays = 0
                    };
                }

                decimal baseFee = 15.00m;
                decimal weightFee = (decimal)order.WeightKg * 2.50m;
                // İndirim oranı hardcoded — değişirse tüm metot güncellenmeli
                decimal discount = 0.40m;
                decimal totalCost = (baseFee + weightFee) * (1 - discount);

                return new ShippingResult
                {
                    IsSuccess = true,
                    Message = "Premium üye indirimi uygulandı.",
                    Cost = totalCost,
                    CarrierName = "PTT Kargo",
                    EstimatedDays = 4
                };
            }
            else
            {
                // Bilinmeyen tip için son çare — yeni tip eklenince buraya düşebilir
                return new ShippingResult
                {
                    IsSuccess = false,
                    Message = $"Bilinmeyen kargo tipi: {order.ShippingType}",
                    Cost = 0,
                    CarrierName = string.Empty,
                    EstimatedDays = 0
                };
            }
        }
    }
}