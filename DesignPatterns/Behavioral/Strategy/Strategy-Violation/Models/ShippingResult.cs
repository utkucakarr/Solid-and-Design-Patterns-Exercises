namespace Strategy_Violation.Models
{
    public class ShippingResult
    {
        // IsSuccess, Message, Cost, CarrierName, EstimatedDays, StrategyUsed

        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public int EstimatedDays { get; set; }
    }
}