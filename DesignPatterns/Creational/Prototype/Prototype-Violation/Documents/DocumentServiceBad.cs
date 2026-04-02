namespace Prototype_Violation.Documents
{
    // Her belge sıfırdan oluşturuluyor — şablon yok, kopyalama yok
    public class DocumentServiceBad
    {
        public ReportDocumentBad CreateMothlyReport(string month)
        {
            var reportDocument = new ReportDocumentBad(
                title: $"{month} Aylık Raporu",
                content: "Standart rapor içeriği", // Tekrar eden şablon
                tableDate: new List<string> // Her seferinde aynı liste
                {
                    "Gelir",
                    "Gider",
                    "Net Kar",
                    "KDV"
                },
                author: "Sistem");

            return reportDocument;
        }

        public ReportDocumentBad CreateWeeklyReport(string week)
        {
            var reportDocumnet = new ReportDocumentBad(
                title: $"{week} Haftalık Raporu",
                content: "Standart rapor içeriği",
                tableDate: new List<string>
                {
                                        "Gelir",
                    "Gider",
                    "Net Kar",
                    "KDV"
                },
                author: "Sistem");
            return reportDocumnet;
        }
    }
}