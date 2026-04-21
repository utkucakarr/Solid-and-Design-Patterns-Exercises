using System.Data;
using System.Net.Http.Headers;
using TemplateMethod_Implementation.Interfaces;
using TemplateMethod_Implementation.Models;

namespace TemplateMethod_Implementation.Reports
{
    // Template Method pattern'inin kalbi: algoritma iskeleti burada sabitlendi
    // Ortak adımlar (FetchData, Validate, Log) bir kez yazıldı, tüm subclass'lar kullanır
    // Değişen adımlar (FormatHeader, FormatRows, FormatFooter) abstract olarak tanımlandı
    public abstract class ReportGeneratorBase : IReportGenerator
    {
        // Metot 'virtual' olmadığı için alt sınıflar (subclasses) bunu override edemez. (Template Method için tam istediğimiz şey)
        public ReportResult Generate(string reportTitle, IEnumerable<string> data)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(reportTitle, nameof(reportTitle));
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            // Adım 1: Veriyi getir (ortak)
            var rows = FetchData(data);

            // Adım 2: Doğrula (ortak)
            var validationError = Validate(rows);
            if (validationError is not null)
                return ReportResult.Fail(validationError);

            // Adım 3-5: Formatla (her subclass farklı uygular)
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(FormatHeader(reportTitle));
            sb.AppendLine(FormatRows(rows));
            sb.AppendLine(FormatFooter(reportTitle, rows.Count));

            // Adım 6: Log at (ortak)
            Log(reportTitle);

            return ReportResult.Success(sb.ToString(), FormatName, reportTitle);
        }

        // Ortak adım — tüm subclass'lar bu implementasyonu kullanır
        protected virtual List<string> FetchData(IEnumerable<string> data) => data.ToList();

        // Ortak adım - merkezi doğrulama, bir ke değişir, hepsi güncellenir.
        protected virtual string? Validate(List<string> rows) =>
            rows.Count == 0 ? "Rapor verisi boş olamaz." : null;

        // Ortak adım - loglama formatı merkezi tutarsızlık yok
        protected virtual void Log(string reportTitle) =>
            Console.WriteLine($"[LOG] {FormatName} raporu oluşturuldu: {reportTitle} ({DateTime.UtcNow:HH:mm:ss})");

        // Abstract adımlar — subclass'ların uygulaması zorunlu
        protected abstract string FormatName { get; }
        protected abstract string FormatHeader(string reportTitle);
        protected abstract string FormatRows(List<string> rows);
        protected abstract string FormatFooter(string reportTitle, int rowCount);
    }
}