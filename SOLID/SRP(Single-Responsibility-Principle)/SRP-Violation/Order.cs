namespace SRP_Violation
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();

        public decimal CalculateTotal()
        => Items.Sum(i => i.Price * i.Quantity);

        public class OrderItem
        {
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }

        // 2. Veritabanı işi — BURAYA AİT DEĞİL!
        public void SaveToDatabase()
            => Console.WriteLine($"[DB] Order {Id} kaydedildi.");

        // 3. E-posta işi — BURAYA AİT DEĞİL!
        public void SendConfirmationEmail()
            => Console.WriteLine($"[EMAIL] {CustomerEmail} adresine onay gönderildi.");

        // 4. Loglama işi — BURAYA AİT DEĞİL!
        public void LogOrder()
            => Console.WriteLine($"[LOG] Order {Id} işlendi. Tarih: {DateTime.Now}");
    }
}
