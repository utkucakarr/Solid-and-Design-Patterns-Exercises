namespace Prototype_Violation.Documents
{
    public class ReportDocumentBad
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> TableData { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Author { get; set; }

        public ReportDocumentBad(
            string title,
            string content,
            List<string> tableDate,
            string author)
        {
            Title = title;
            Content = content;
            TableData = tableDate;
            Author = author;
            CreatedAt = DateTime.Now;

            //  Simüle edilmiş ağır işlem — gerçekte DB'den veri çekme,
            //  şablon yükleme, format dönüşümü gibi maliyetli adımlar olabilir
            Console.WriteLine($"[ReportDocumentBad] '{title}' sıfırdan oluşturuluyor..." +
                  " (Ağır işlem simüle ediliyor)");
            Thread.Sleep(100);
        }
    }
}
