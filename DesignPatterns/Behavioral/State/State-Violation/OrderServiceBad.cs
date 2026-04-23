namespace State_Violation
{
    // Tüm state yönetimi tek bir sınıfta toplanmış
    // Her yeni durum eklendikçe bu sınıf büyümeye devam eder (OCP ihlali)
    public class OrderServiceBad
    {
        public string OrderId { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal Amount { get; private set; }

        // Constructor'da doğrudan nesne oluşturuluyor, DI yok
        public OrderServiceBad(string orderId, decimal amount)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(orderId, nameof(orderId));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));

            OrderId = orderId;
            Amount = amount;
            Status = OrderStatus.Pending;
        }

        // Her metot tüm state'leri if/switch ile kontrol ediyor
        // Yeni state eklemek tüm metodları değiştirmeyi gerektiriyor
        public string Confirm()
        {
            if (Status == OrderStatus.Pending)
            {
                Status = OrderStatus.Confirmed;
                return $"Sipariş {OrderId} onaylandı.";
            }
            else if (Status == OrderStatus.Confirmed)
            {
                // Tekrar onaylama durumu kontrol ediliyor
                return $"Sipariş {OrderId} zaten onaylanmış.";
            }
            else if (Status == OrderStatus.Shipped)
            {
                return $"Sipariş {OrderId} kargoda, onaylanamaz.";
            }
            else if (Status == OrderStatus.Delivered)
            {
                return $"Sipariş {OrderId} teslim edildi, onaylanamaz.";
            }
            else if (Status == OrderStatus.Cancelled)
            {
                return $"Sipariş {OrderId} iptal edildi, onaylanamaz.";
            }

            // Bu else bloğu hiçbir zaman tetiklenmemeli ama mecburen yazılıyor
            return "[!] Bilinmeyen durum.";
        }

        // Ship metodu da aynı if/switch karmaşasını tekrarlıyor
        public string Ship()
        {
            if (Status == OrderStatus.Pending)
            {
                return $"Sipariş {OrderId} henüz onaylanmadı, kargoya verilemez.";
            }
            else if (Status == OrderStatus.Confirmed)
            {
                Status = OrderStatus.Shipped;
                return $"Sipariş {OrderId} kargoya verildi.";
            }
            else if (Status == OrderStatus.Shipped)
            {
                return $"Sipariş {OrderId} zaten kargoda.";
            }
            else if (Status == OrderStatus.Delivered)
            {
                return $"Sipariş {OrderId} teslim edildi, kargoya verilemez.";
            }
            else if (Status == OrderStatus.Cancelled)
            {
                return $"Sipariş {OrderId} iptal edildi, kargoya verilemez.";
            }

            return "Bilinmeyen durum.";
        }

        // Deliver metodu da aynı pattern'i tekrarlıyor
        public string Deliver()
        {
            if (Status == OrderStatus.Pending)
            {
                return $"Sipariş {OrderId} beklemede, teslim edilemez.";
            }
            else if (Status == OrderStatus.Confirmed)
            {
                return $"Sipariş {OrderId} onaylandı ama kargoya verilmedi.";
            }
            else if (Status == OrderStatus.Shipped)
            {
                Status = OrderStatus.Delivered;
                return $"Sipariş {OrderId} teslim edildi.";
            }
            else if (Status == OrderStatus.Delivered)
            {
                return $"Sipariş {OrderId} zaten teslim edildi.";
            }
            else if (Status == OrderStatus.Cancelled)
            {
                return $"Sipariş {OrderId} iptal edildi, teslim edilemez.";
            }

            return "Bilinmeyen durum.";
        }

        // Cancel metodu da büyüdükçe büyüyor
        public string Cancel()
        {
            if (Status == OrderStatus.Pending)
            {
                Status = OrderStatus.Cancelled;
                return $"Sipariş {OrderId} iptal edildi (beklemedeydi).";
            }
            else if (Status == OrderStatus.Confirmed)
            {
                Status = OrderStatus.Cancelled;
                return $"Sipariş {OrderId} iptal edildi (onaylanmıştı).";
            }
            else if (Status == OrderStatus.Shipped)
            {
                // İş kuralı: kargodaki sipariş iptal edilemez ama bu kural burada gömülü
                return $"Sipariş {OrderId} kargoda, iptal edilemez.";
            }
            else if (Status == OrderStatus.Delivered)
            {
                return $"Sipariş {OrderId} teslim edildi, iptal edilemez.";
            }
            else if (Status == OrderStatus.Cancelled)
            {
                return $"Sipariş {OrderId} zaten iptal edilmiş.";
            }

            return "Bilinmeyen durum.";
        }

        // Yeni bir "ReturnRequested" state'i eklemek istersen
        // bu 4 metodun HEPSİNE yeni if bloğu eklenmesi gerekir
        public string GetStatusDescription()
        {
            // String map de ayrıca burada tekrar if/switch
            return Status switch
            {
                OrderStatus.Pending => "Ödeme bekleniyor",
                OrderStatus.Confirmed => "Sipariş onaylandı",
                OrderStatus.Shipped => "Kargoya verildi",
                OrderStatus.Delivered => "Teslim edildi",
                OrderStatus.Cancelled => "İptal edildi",
                _ => "Bilinmiyor"
            };
        }
    }
}
