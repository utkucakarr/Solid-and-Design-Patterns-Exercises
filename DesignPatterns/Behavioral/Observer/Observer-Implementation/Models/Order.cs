namespace Observer_Implementation.Models
{
    public class Order
    {
        public string OrderId { get; init; }
        public string CustomerEmail { get; init; }
        public string CustomerPhone { get; init; }
        public string CustomerDeviceToken { get; init; }
        public string ProductName { get; init; }
        public int Quantity { get; init; }
        public decimal TotalPrice { get; init; }
        public OrderStatus Status { get; private set; }


        public Order(
        string orderId,
        string customerEmail,
        string customerPhone,
        string customerDeviceToken,
        string productName,
        int quantity,
        decimal totalPrice)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(orderId, nameof(orderId));
            ArgumentException.ThrowIfNullOrWhiteSpace(customerEmail, nameof(customerEmail));
            ArgumentException.ThrowIfNullOrWhiteSpace(customerPhone, nameof(customerPhone));
            ArgumentException.ThrowIfNullOrWhiteSpace(customerDeviceToken, nameof(customerDeviceToken));
            ArgumentException.ThrowIfNullOrWhiteSpace(productName, nameof(productName));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(totalPrice, nameof(totalPrice));

            OrderId = orderId;
            CustomerEmail = customerEmail;
            CustomerPhone = customerPhone;
            CustomerDeviceToken = customerDeviceToken;
            ProductName = productName;
            Quantity = quantity;
            TotalPrice = totalPrice;
            Status = OrderStatus.Placed;
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }
    }
}
