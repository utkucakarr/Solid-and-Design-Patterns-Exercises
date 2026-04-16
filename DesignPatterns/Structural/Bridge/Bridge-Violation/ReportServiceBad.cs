namespace Bridge_Violation
{
    // Her rapor tiği * fotmat kombinasyonu için ayrı sınıf - class explosion!

    // Satış Raporu - 3 formar = 3 sınıf

    public class SalesPdfReport_Bad
    {
        public string Generate()
        {
            // Satış verisi toplama mantığı burada
            var data = "Toplam Satış: 150.000 TL, Adet: 320";
            // PDF render mantığı burada — format ve içerik iç içe!
            Console.WriteLine("[PDF] Satış raporu PDF olarak oluşturuluyor.");
            return $"[PDF]{data}[/PDF]";
        }
    }

    public class SalesExcelReport_Bad
    {
        public string Generate()
        {
            // Aynı satış verisi burada da tekrar yazıldı — DRY ihlali!
            var data = "Toplam Satış: 150.000 TL, Adet: 320";
            Console.WriteLine("[Excel] Satış raporu Excel olarak oluşturuluyor.");
            return $"<excel>{data}</excel>";
        }
    }

    public class SalesCsvReport_Bad
    {
        public string Generate()
        {
            // Aynı satış verisi 3. kez yazıldı!
            var data = "Toplam Satış: 150.000 TL, Adet: 320";
            Console.WriteLine("[CSV] Satış raporu CSV olarak oluşturuluyor.");
            return $"date,sales,count\n2024-01,150000,320";
        }
    }

    // Stok Raporu — 3 format = 3 sınıf daha
    public class StockPdfReport_Bad
    {
        public string Generate()
        {
            var data = "Toplam Stok: 5.420 Ürün, Kritik: 12";
            Console.WriteLine("[PDF] Stok raporu PDF olarak oluşturuluyor.");
            return $"[PDF]{data}[/PDF]";
            // PDF render kodu SalesPdfReport_Bad ile aynı — DRY ihlali!
        }
    }

    public class StockExcelReport_Bad
    {
        public string Generate()
        {
            var data = "Toplam Stok: 5.420 Ürün, Kritik: 12";
            Console.WriteLine("[Excel] Stok raporu Excel olarak oluşturuluyor.");
            return $"<excel>{data}</excel>";
        }
    }

    public class StockCsvReport_Bad
    {
        public string Generate()
        {
            var data = "Toplam Stok: 5.420 Ürün, Kritik: 12";
            Console.WriteLine("[CSV] Stok raporu CSV olarak oluşturuluyor.");
            return $"product,stock,critical\nA101,5420,12";
        }
    }

    // Finans Raporu — 3 format = 3 sınıf daha
    public class FinancePdfReport_Bad
    {
        public string Generate()
        {
            var data = "Gelir: 500.000 TL, Gider: 320.000 TL, Kâr: 180.000 TL";
            Console.WriteLine("[PDF] Finans raporu PDF olarak oluşturuluyor.");
            return $"[PDF]{data}[/PDF]";
        }
    }

    public class FinanceExcelReport_Bad
    {
        public string Generate()
        {
            var data = "Gelir: 500.000 TL, Gider: 320.000 TL, Kâr: 180.000 TL";
            Console.WriteLine("[Excel] Finans raporu Excel olarak oluşturuluyor.");
            return $"<excel>{data}</excel>";
        }
    }

    public class FinanceCsvReport_Bad
    {
        public string Generate()
        {
            var data = "Gelir: 500.000 TL, Gider: 320.000 TL, Kâr: 180.000 TL";
            Console.WriteLine("[CSV] Finans raporu CSV olarak oluşturuluyor.");
            return $"income,expense,profit\n500000,320000,180000";
        }
    }

    // 3 rapor × 3 format = 9 sınıf
    // Yeni format (HTML) = 3 yeni sınıf daha — tüm rapor tiplerine dokun!
    // Yeni rapor tipi (Müşteri) = 3 yeni sınıf daha — tüm formatlara dokun!
    // 4 rapor × 4 format = 16 sınıf — class explosion!
}
