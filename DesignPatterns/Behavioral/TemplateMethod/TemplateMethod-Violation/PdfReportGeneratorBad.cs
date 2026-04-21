namespace TemplateMethod_Violation
{
    // Her rapor sınıfı kendi Generate metodunu tamamen kendisi yazıyor
    // Ortak adımlar (veri getirme, doğrulama, loglama) her sınıfta tekrar ediyor
    // Algoritma iskeleti hiçbir yerde merkezi olarak tanımlanmıyor
    // Yeni bir adım eklenmek istendiğinde tüm sınıflar tek tek değiştirilmeli
    public class PdfReportGeneratorBad
    {
        public string Generate(string reportTitle, IEnumerable<string> data)
        {
            // Veri getirme mantığı doğrudan burada — diğer sınıflarda da birebir aynı
            var rows = data.ToList();
            if (!rows.Any())
                return "Hata: Veri bulunamadı.";

            // Doğrulama her sınıfta ayrı ayrı yazıldı, tutarsızlıklar kaçınılmaz
            if (string.IsNullOrWhiteSpace(reportTitle))
                return "Hata: Başlık boş olamaz.";

            var sb = new System.Text.StringBuilder();

            // PDF'e özgü formatlama — bu kısım farklı olmalı ama üstündeki adımlar aynı
            sb.AppendLine("[PDF HEADER]");
            sb.AppendLine($"Başlık: {reportTitle}");
            sb.AppendLine("---");
            foreach (var row in rows)
                sb.AppendLine($" {row}");
            sb.AppendLine("[PDF FOOTER] — Gizlidir");

            // Loglama her sınıfta tekrar ediyor, format farklılaşabilir
            Console.WriteLine($"[LOG] PDF raporu oluşturuldu: {reportTitle}");

            return sb.ToString();
        }
    }
}
