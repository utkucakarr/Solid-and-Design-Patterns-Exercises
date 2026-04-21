namespace TemplateMethod_Implementation.Reports
{
    // Sadece CSV'e özgü virgüllü format burada
    public class CsvReportGenerator : ReportGeneratorBase
    {
        protected override string FormatName => "CSV";

        protected override string FormatHeader(string reportTitle) =>
            $"Başlık,Değer\nRapor,{reportTitle}";

        protected override string FormatRows(List<string> rows)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var item in rows)
                sb.AppendLine($"Satır,{item}");
            return sb.ToString().TrimEnd();
        }

        protected override string FormatFooter(string reportTitle, int rowCount) =>
            $"Özet,{rowCount} kayıt dışa aktarıldı";
    }
}
