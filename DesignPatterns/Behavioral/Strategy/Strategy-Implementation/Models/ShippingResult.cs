namespace Strategy_Implementation.Models
{
    public sealed class ShippingResult
    {
        public bool IsSuccess { get; private init; }
        public string Message { get; private init; } = string.Empty;
        public decimal Cost { get; private init; }
        public string CarrierName { get; private init; } = string.Empty;
        public int EstimatedDays { get; private init; }
        public string StrategyUsed { get; private init; } = string.Empty;

        private ShippingResult(){ }

        // Static Factory - başarılı senaryo
        public static ShippingResult Success(
            decimal cost,
            int estimatedDays,
            string carrierName,
            string strategyUsed,
            string message = "Kargo ücreti başarıyla hesaplandı.") =>
            new()
            {
                Message = message,
                IsSuccess = true,
                Cost = cost,
                CarrierName = carrierName,
                EstimatedDays = estimatedDays,
                StrategyUsed = strategyUsed
            };

        // Static Factory - başarısız senaryo
        public static ShippingResult Fail(string reason) =>
            new()
            {
                IsSuccess = false,
                Message = reason,
                Cost = 0,
                CarrierName = string.Empty,
                EstimatedDays = 0,
                StrategyUsed = string.Empty
            };
    }
}