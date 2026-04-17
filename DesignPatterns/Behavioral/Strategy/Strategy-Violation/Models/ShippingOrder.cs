namespace Strategy_Violation.Models
{
    public class ShippingOrder
    {
        public string OrderId { get; set; } = string.Empty;
        public double WeightKg { get; set; }
        public decimal OrderTotal { get; set; }
        public string MemberShipType { get; set; } = string.Empty; // "standard", "premium"
        public string ShippingType { get; set; } = string.Empty; // "standard", "express", "free", "member"
    }
}
