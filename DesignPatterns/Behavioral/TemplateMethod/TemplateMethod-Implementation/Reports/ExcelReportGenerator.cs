
namespace TemplateMethod_Implementation.Reports
{
    // Sadece Excel'e özgü hücre formatı burada
    public sealed class ExcelReportGenerator : ReportGeneratorBase
    {
        protected override string FormatName => "Excel";

        protected override string FormatFooter(string reportTitle, int rowCount) =>
        $"HÜCRE[Son]: Toplam Satır: {rowCount}";

        protected override string FormatHeader(string reportTitle) =>
        $"HÜCRE[A1]: {reportTitle}\nHÜCRE[A2]: Veri";

        protected override string FormatRows(List<string> rows)
        {
            var sb = new System.Text.StringBuilder();
            int rowIndex = 3;
            foreach (var item in rows)
                sb.AppendLine($"HÜCRE[A{rowIndex++}]: {item}");
            return sb.ToString().TrimEnd();
        }
    }
}
