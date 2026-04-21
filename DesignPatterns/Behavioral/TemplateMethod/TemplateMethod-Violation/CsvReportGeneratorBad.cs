namespace TemplateMethod_Violation
{
    // Üçüncü sınıf, üçüncü kopya — algoritma iskeleti tamamen dağınık
    // Bir geliştirici doğrulama kuralını değiştirirse 3 dosyayı bulup güncellenmeli
    // Gözden kaçan bir dosya -> production'da tutarsız davranış
    public class CsvReportGeneratorBad
    {
        public string Generate(string reportTitle, IEnumerable<string> data)
        {
            // Aynı kontrol — üçüncü kez
            var rows = data.ToList();
            if (!rows.Any())
                return "Hata: Veri bulunamadı.";

            if (string.IsNullOrWhiteSpace(reportTitle))
                return "Hata: Başlık boş olamaz.";

            var sb = new System.Text.StringBuilder();

            // Yalnızca bu kısım gerçekten CSV'e özgü
            sb.AppendLine("Başlık,Değer");
            sb.AppendLine($"Rapor,{reportTitle}");
            foreach (var item in rows)
                sb.AppendLine($"Satır,{item}");

            // Bu sınıfta log hiç atılmadı — geliştirici unuttu, fark edilmedi
            return sb.ToString();
        }
    }
}
