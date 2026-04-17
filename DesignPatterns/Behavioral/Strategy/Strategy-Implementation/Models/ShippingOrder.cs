namespace Strategy_Implementation.Models
{
    public class ShippingOrder
    {
        public string OrderId { get; set; } = string.Empty;
        public double WeightKg { get; set; }
        public decimal OrderTotal { get; set; }
        public string MembershipType { get; set; } = string.Empty;
    }
}
