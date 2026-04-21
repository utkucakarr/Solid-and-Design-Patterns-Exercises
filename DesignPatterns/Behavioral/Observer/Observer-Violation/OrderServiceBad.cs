namespace Observer_Violation
{
    public class OrderServiceBad
    {
        // Tüm bildirim ve iş mantığı tek sınıfa gömülmüş - SRP ihlali

        private readonly List<Order> _orders = new ();

        public void ChangeOderStatus(string orderId, OrderStatus newStatus)
        {
            var order = _orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
                return;

            order.Status = newStatus;

            // Her durum değişiminde tüm if/switch zincirleri elle çalıştırılıyor
            // Yeni bir bildirim kanalı (WhatsApp, Slack vb.) eklemek bu metodu şişirir

            if (newStatus == OrderStatus.Confirmed)
            {
                // E-posta gönderimi doğrudan burada — ayrı servis yok
                Console.WriteLine($"[EMAIL] Merhaba {order.CustomerEmail}, " +
                    $"siparişiniz #{order.OrderId} onaylandı.");

                // SMS gönderimi doğrudan burada — ayrı servis yok
                Console.WriteLine($"[SMS] {order.CustomerPhone} numarasına mesaj: " +
                    $"Siparişiniz #{order.OrderId} onaylandı.");

                // Stok düşümü burada yapılıyor — envanter servisi yok
                Console.WriteLine($"[INVENTORY] {order.ProductName} için " +
                    $"{order.Quantity} adet stok rezerve edildi.");
            }
            else if (newStatus == OrderStatus.Shipped)
            {
                // Durum değiştikçe aynı if zinciri büyümeye devam ediyor
                Console.WriteLine($"[EMAIL] Merhaba {order.CustomerEmail}, " +
                    $"siparişiniz #{order.OrderId} kargoya verildi.");

                Console.WriteLine($"[SMS] {order.CustomerPhone} numarasına mesaj: " +
                    $"Siparişiniz #{order.OrderId} kargoya verildi.");

                // 🚨 Push bildirimi de doğrudan buraya yazılmış
                Console.WriteLine($"[PUSH] {order.CustomerDeviceToken} cihazına bildirim: " +
                    $"Siparişiniz yola çıktı!");
            }
            else if (newStatus == OrderStatus.Delivered)
            {
                Console.WriteLine($"[EMAIL] Merhaba {order.CustomerEmail}, " +
                    $"siparişiniz #{order.OrderId} teslim edildi.");

                Console.WriteLine($"[SMS] {order.CustomerPhone} numarasına mesaj: " +
                    $"Siparişiniz #{order.OrderId} teslim edildi.");

                Console.WriteLine($"[PUSH] {order.CustomerDeviceToken} cihazına bildirim: " +
                    $"Siparişiniz teslim edildi!");

                // Fatura oluşturma da bu metoda sıkıştırılmış
                Console.WriteLine($"[INVOICE] #{order.OrderId} için " +
                    $"{order.TotalPrice:C} tutarında fatura oluşturuldu.");
            }
            else if (newStatus == OrderStatus.Cancelled)
            {
                Console.WriteLine($"[EMAIL] Merhaba {order.CustomerEmail}, " +
                    $"siparişiniz #{order.OrderId} iptal edildi.");

                Console.WriteLine($"[SMS] {order.CustomerPhone} numarasına mesaj: " +
                    $"Siparişiniz #{order.OrderId} iptal edildi.");

                // Stok iadesi de aynı metotta — bağımlılık zincirine yenisi eklendi
                Console.WriteLine($"[INVENTORY] {order.ProductName} için " +
                    $"{order.Quantity} adet stok iade edildi.");
            }
            // Yeni durum (Processing, ReturnRequested vb.) eklemek bu metodu daha da uzatır
        }

        public void PlaceOrder(Order order)
        {
            // Order nesnesi doğrudan listeye ekleniyor, hiçbir doğrulama yok
            _orders.Add(order);

            Console.WriteLine($"[EMAIL] Merhaba {order.CustomerEmail}, " +
                $"siparişiniz #{order.OrderId} alındı.");

            Console.WriteLine($"[SMS] {order.CustomerPhone} numarasına mesaj: " +
                $"Siparişiniz #{order.OrderId} alındı.");

            // Stok kontrolü ve fatura da PlaceOrder içinde karışık
            Console.WriteLine($"[INVENTORY] {order.ProductName} için " +
                $"stok kontrolü yapıldı.");
        }
    }
}
