namespace SRP_Implementation.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();

        public decimal CalculateTotal()
        => Items.Sum(i => i.Price * i.Quantity);
    }
}
