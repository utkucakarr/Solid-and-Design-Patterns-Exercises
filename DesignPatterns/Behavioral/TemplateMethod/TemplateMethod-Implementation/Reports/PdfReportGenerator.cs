using System.Text;

namespace TemplateMethod_Implementation.Reports
{
    // Sadece PDF'e özgü adımlar burada — ortak mantık base class'ta
    public sealed class PdfReportGenerator : ReportGeneratorBase
    {
        protected override string FormatName => "PDF";

        // PDF footer formatı
        protected override string FormatFooter(string reportTitle, int rowCount) => $"{'='.ToString().PadRight(40, '=')}\n[PDF FOOTER] Toplam: {rowCount} kayıt — Gizlidir";

        // PDF başlık formatı
        protected override string FormatHeader(string reportTitle) =>
            $"[PDF HEADER]\nBaşlık: {reportTitle}\n{'='.ToString().PadRight(40, '=')}";

        // Pdf satır formatı
        protected override string FormatRows(List<string> rows)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var row in rows)
                sb.AppendLine($" {row}");
            return sb.ToString().TrimEnd();
        }
    }
}
