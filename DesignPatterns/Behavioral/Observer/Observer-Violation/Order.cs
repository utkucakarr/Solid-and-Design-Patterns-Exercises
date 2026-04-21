namespace Observer_Violation
{
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerDeviceToken { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }

        public Order(string orderId, string customerEmail, string customerPhone,
    string customerDeviceToken, string productName, int quantity, decimal totalPrice)
        {
            OrderId = orderId;
            CustomerEmail = customerEmail;
            CustomerPhone = customerPhone;
            CustomerDeviceToken = customerDeviceToken;
            ProductName = productName;
            Quantity = quantity;
            TotalPrice = totalPrice;
            Status = OrderStatus.Placed;
        }

    }
}
